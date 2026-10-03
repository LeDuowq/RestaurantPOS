namespace BusinessObject
{
    public class BankItem
    {
        public string BankId { get; set; } = "";
        public string BankName { get; set; } = "";

        public override string ToString() => BankName;
    }

    public class BankingConfig
    {
        public string BankId { get; set; } = "MB";
        public string BankName { get; set; } = "MB Bank (Ngân Hàng TMCP Quân Đội)";
        public string AccountNumber { get; set; } = "0384240888";
        public string AccountName { get; set; } = "LE DUC DUONG";
        public string Template { get; set; } = "compact2";
    }
}
