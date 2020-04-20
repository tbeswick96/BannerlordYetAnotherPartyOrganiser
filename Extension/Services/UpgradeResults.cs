namespace YAPO.Services
{
    public class UpgradeResults
    {
        public int UpgradedTotal { get; set; } = 0;
        public int UpgradedTypes { get; set; } = 0;
        public int MultiPathUpgraded { get; set; } = 0;
        public int MultiPathSkipped { get; set; } = 0;
    }
}
