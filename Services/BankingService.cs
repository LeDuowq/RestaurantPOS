using BusinessObject;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace Services
{
    public class BankingService : IBankingService
    {
        private BankingConfig? _config;

        public BankingConfig GetBankingConfig()
        {
            if (_config != null) return _config;

            try
            {
                string configPath = GetConfigFilePath();
                if (File.Exists(configPath))
                {
                    string jsonContent = File.ReadAllText(configPath);
                    using var doc = JsonDocument.Parse(jsonContent);
                    if (doc.RootElement.TryGetProperty("RestaurantBanking", out var bankingElement))
                    {
                        _config = JsonSerializer.Deserialize<BankingConfig>(bankingElement.GetRawText(), new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true
                        });
                    }
                }
            }
            catch
            {
                // Fallback nếu có lỗi
            }

            _config ??= new BankingConfig();
            return _config;
        }

        public bool SaveBankingConfig(BankingConfig newConfig)
        {
            try
            {
                _config = newConfig;
                string configPath = GetConfigFilePath();

                // Cập nhật file JSON bằng JsonNode để giữ nguyên ConnectionStrings
                if (File.Exists(configPath))
                {
                    string jsonContent = File.ReadAllText(configPath);
                    var rootNode = JsonNode.Parse(jsonContent);
                    if (rootNode != null)
                    {
                        rootNode["RestaurantBanking"] = JsonSerializer.SerializeToNode(newConfig, new JsonSerializerOptions
                        {
                            WriteIndented = true
                        });

                        string updatedJson = rootNode.ToJsonString(new JsonSerializerOptions
                        {
                            WriteIndented = true
                        });

                        File.WriteAllText(configPath, updatedJson);

                        // Đồng thời cập nhật file nguồn trong project nếu đang chạy chế độ debug
                        TryUpdateProjectSourceConfig(updatedJson);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string GenerateVietQrUrl(decimal amount, string description)
        {
            var config = GetBankingConfig();
            long roundedAmount = (long)Math.Max(0, amount);
            string safeDesc = Uri.EscapeDataString(description ?? string.Empty);
            string safeName = Uri.EscapeDataString(config.AccountName ?? string.Empty);
            string template = string.IsNullOrWhiteSpace(config.Template) ? "compact2" : config.Template;

            return $"https://img.vietqr.io/image/{config.BankId}-{config.AccountNumber}-{template}.png?amount={roundedAmount}&addInfo={safeDesc}&accountName={safeName}";
        }

        public List<BankItem> GetPopularBanks()
        {
            return new List<BankItem>
            {
                new BankItem { BankId = "MB", BankName = "MB Bank (Ngân Hàng TMCP Quân Đội)" },
                new BankItem { BankId = "VCB", BankName = "Vietcombank (Ngân Hàng TMCP Ngoại Thương VN)" },
                new BankItem { BankId = "TCB", BankName = "Techcombank (Ngân Hàng Kỹ Thương VN)" },
                new BankItem { BankId = "BIDV", BankName = "BIDV (Ngân Hàng Đầu Tư & Phát Triển VN)" },
                new BankItem { BankId = "ICB", BankName = "VietinBank (Ngân Hàng Công Thương VN)" },
                new BankItem { BankId = "VBA", BankName = "Agribank (Ngân Hàng Nông Nghiệp & PTNT VN)" },
                new BankItem { BankId = "VPB", BankName = "VPBank (Ngân Hàng Việt Nam Thịnh Vượng)" },
                new BankItem { BankId = "ACB", BankName = "ACB (Ngân Hàng Á Châu)" },
                new BankItem { BankId = "TPB", BankName = "TPBank (Ngân Hàng Tiên Phong)" },
                new BankItem { BankId = "STB", BankName = "Sacombank (Ngân Hàng Sài Gòn Thương Tín)" },
                new BankItem { BankId = "HDB", BankName = "HDBank (Ngân Hàng Phát Triển TP.HCM)" },
                new BankItem { BankId = "VIB", BankName = "VIB (Ngân Hàng Quốc Tế)" },
                new BankItem { BankId = "SHB", BankName = "SHB (Ngân Hàng Sài Gòn - Hà Nội)" },
                new BankItem { BankId = "OCB", BankName = "OCB (Ngân Hàng Phương Đông)" },
                new BankItem { BankId = "MSB", BankName = "MSB (Ngân Hàng Hàng Hải)" },
                new BankItem { BankId = "SEAB", BankName = "SeABank (Ngân Hàng Đông Nam Á)" },
                new BankItem { BankId = "LPB", BankName = "LPBank (Ngân Hàng Lộc Phát VN)" }
            };
        }

        private string GetConfigFilePath()
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
        }

        private void TryUpdateProjectSourceConfig(string jsonContent)
        {
            try
            {
                // Thử tìm file appsettings.json trong thư mục nguồn project
                DirectoryInfo? dir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
                while (dir != null)
                {
                    string candidate = Path.Combine(dir.FullName, "Restaurant", "Restaurant", "appsettings.json");
                    if (File.Exists(candidate))
                    {
                        File.WriteAllText(candidate, jsonContent);
                        break;
                    }
                    string candidateDirect = Path.Combine(dir.FullName, "appsettings.json");
                    if (File.Exists(candidateDirect) && dir.Name == "Restaurant")
                    {
                        File.WriteAllText(candidateDirect, jsonContent);
                        break;
                    }
                    dir = dir.Parent;
                }
            }
            catch
            {
                // Bỏ qua nếu không có quyền ghi nguồn
            }
        }
    }
}
