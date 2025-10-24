using System;

namespace solidhardware.storeCore.Domain.Entites
{
    public class ProductSpecialProperty
    {
        public Guid Id { get; set; }  // لازم مفتاح أساسي
        public Guid ProductId { get; set; }
        public Product Product { get; set; }

        // SSD
        public int? SSD_Capacity { get; set; }
        public string? CapacityUnit { get; set; }
        public string? SSD_Type { get; set; }

        // Case
        public string? Case_Power_Supply { get; set; }
        public string? Fans_Cooling { get; set; }
        public string? Color { get; set; }

        // GPU
        public string? CPU_Chip { get; set; }
        public string? CPU_Series { get; set; }
        public int? VRAMSize { get; set; }
        public string? VRAMUnit { get; set; } = "GB";

        // HDD
        public int? HDD_Capacity { get; set; }

        // Motherboard
        public string? Motherboard_Chipset { get; set; }
        public string? CPU_Socket { get; set; }

        // RAM
        public string? RAM_Type { get; set; }
        public int? Capacity { get; set; }
        public int? ModuleCount { get; set; }
        public int? Speed { get; set; }
        public string? SpeedUnit { get; set; } = "MHz";

        // Monitor
        public string? PanelType { get; set; }
        public int? RefreshRate { get; set; }
        public string? RefreshRate_Unit { get; set; } = "Hz";
        public string? Resolution { get; set; }
        public decimal? ResponseTime { get; set; }
        public string? ResponseTimeUnit { get; set; } = "ms";
        public decimal? ScreenSize { get; set; }
        public string? ScreenSizeUnit { get; set; } = "Inch";

        // PSU
        public string? PSU_Wattage { get; set; }
    }
}
