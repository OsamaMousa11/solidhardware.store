using AutoMapper;
using Microsoft.Extensions.Logging;
using solidhardware.storeCore.Domain.Entites;
using solidhardware.storeCore.Domain.IRepositoryContract;
using solidhardware.storeCore.DTO.OrderDTO;
using solidhardware.storeCore.Helper;
using solidhardware.storeCore.IUnitofWork;
using solidhardware.storeCore.ServiceContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace solidhardware.storeCore.Service
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderService> _logger;
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;

        public OrderService(IUnitOfWork unitOfWork, ILogger<OrderService> logger, IMapper mapper , IOrderRepository orderRepository)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _orderRepository = orderRepository;
        }

        // --------------------------------------------------
        // CREATE
        // --------------------------------------------------
        public async Task<OrderResponse> CreateOrderAsync(OrderAddRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            ValidationHelper.ValidateModel(request);
            _logger.LogInformation("Creating order for user {UserId}", request.UserId);

            var order = _mapper.Map<Order>(request);
            order.Id = Guid.NewGuid();
            order.OrderDate = DateTime.UtcNow;

            if (request.OrderItems != null && request.OrderItems.Any())
            {
                order.OrderItems = request.OrderItems.Select(item =>
                {
                    var entity = _mapper.Map<OrderItem>(item);
                    entity.Id = Guid.NewGuid();
                    entity.OrderId = order.Id;
                    return entity;
                }).ToList();
            }

            await _unitOfWork.Repository<Order>().CreateAsync(order);
            await _unitOfWork.CompleteAsync();

            var createdOrder = await _unitOfWork.Repository<Order>()
                .GetByAsync(o => o.Id == order.Id, includeProperties: "OrderItems.Product");

            return _mapper.Map<OrderResponse>(createdOrder);
        }

        // --------------------------------------------------
        // GET BY ID
        // --------------------------------------------------
        public async Task<OrderResponse?> GetOrderByIdAsync(Guid orderId)
        {
            if (orderId == Guid.Empty)
                throw new ArgumentException("Order id cannot be empty.");

            _logger.LogInformation("Fetching order {OrderId}", orderId);

            var order = await _unitOfWork.Repository<Order>()
                .GetByAsync(o => o.Id == orderId, includeProperties: "OrderItems.Product");

            if (order == null)
            {
                _logger.LogWarning("Order {OrderId} not found", orderId);
                return null;
            }

            return _mapper.Map<OrderResponse>(order);
        }

        // --------------------------------------------------
        // GET ALL
        // --------------------------------------------------
        public async Task<List<OrderResponse>> GetAllOrdersAsync()
        {
            _logger.LogInformation("Fetching all orders");

            var orders = await _unitOfWork.Repository<Order>()
                .GetAllAsync(includeProperties: "OrderItems.Product");

            return _mapper.Map<List<OrderResponse>>(orders);
        }

        // --------------------------------------------------
        // UPDATE
        // --------------------------------------------------
        public async Task<OrderResponse> UpdateOrderAsync(OrderUpdateRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            ValidationHelper.ValidateModel(request);
            _logger.LogInformation("Updating order {OrderId}", request.Id);

            var order = await _unitOfWork.Repository<Order>()
                .GetByAsync(o => o.Id == request.Id, includeProperties: "OrderItems");

            if (order == null)
                throw new KeyNotFoundException($"Order with ID {request.Id} not found.");

            // Map main order properties
            _mapper.Map(request, order);

            // -----------------------------
            // UPDATE & ADD order items
            // -----------------------------
            if (request.OrderItems != null)
            {
                foreach (var itemDto in request.OrderItems)
                {
                    var existing = order.OrderItems.FirstOrDefault(i => i.Id == itemDto.Id);

                    if (existing != null)
                    {
                        // Update existing item
                        _mapper.Map(itemDto, existing);
                    }
                    else
                    {
                        // Add new item
                        var newItem = _mapper.Map<OrderItem>(itemDto);
                        newItem.Id = Guid.NewGuid();
                        newItem.OrderId = order.Id;

                        await _unitOfWork.Repository<OrderItem>().CreateAsync(newItem);
                    }
                }
            }

            await _orderRepository.UpdateAsync(order);
            await _unitOfWork.CompleteAsync();

            return _mapper.Map<OrderResponse>(order);
        }


        public async Task<bool> DeleteOrderAsync(Guid orderId)
        {
            if (orderId == Guid.Empty)
                throw new ArgumentException("Order id cannot be empty.");

            _logger.LogInformation("Deleting order {OrderId}", orderId);

            var order = await _unitOfWork.Repository<Order>()
                .GetByAsync(o => o.Id == orderId, includeProperties: "OrderItems");

            if (order == null)
                throw new KeyNotFoundException($"Order with ID {orderId} not found.");

            
            if (order.OrderItems != null && order.OrderItems.Any())
                await _unitOfWork.Repository<OrderItem>().RemoveRangeAsync(order.OrderItems);

            bool deleted = await _unitOfWork.Repository<Order>().DeleteAsync(order);
            await _unitOfWork.CompleteAsync();

            return deleted;
        }
        public async Task<IEnumerable<OrderResponse>> GetOrdersByUserIdAsync(Guid userId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("Invalid user id");

            var orders = await _unitOfWork.Repository<Order>()
                .GetAllAsync(
                    filter: o => o.UserId == userId,
                    includeProperties: "OrderItems.Product"
                );

            return _mapper.Map<IEnumerable<OrderResponse>>(orders);
        }
    }
}
