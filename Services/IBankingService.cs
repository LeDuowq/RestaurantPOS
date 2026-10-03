using BusinessObject;
using System.Collections.Generic;

namespace Services
{
    public interface IBankingService
    {
        BankingConfig GetBankingConfig();
        bool SaveBankingConfig(BankingConfig newConfig);
        string GenerateVietQrUrl(decimal amount, string description);
        List<BankItem> GetPopularBanks();
    }
}
