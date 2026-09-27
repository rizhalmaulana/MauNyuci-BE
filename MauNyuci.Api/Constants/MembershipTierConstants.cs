namespace MauNyuci.Api.Constants
{
    public static class MembershipTierConstants
    {
        public const string Regular = "Regular";
        public const string Premium = "Premium";
        
        // Konstanta ID Statik untuk seeding agar Guid-nya selalu tetap (konsisten) di setiap migrasi
        public static readonly Guid RegularId = Guid.Parse("11111111-2222-3333-4444-111111111111");
        public static readonly Guid PremiumId = Guid.Parse("11111111-2222-3333-4444-111111111112");
    }
}
