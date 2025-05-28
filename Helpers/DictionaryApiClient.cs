using System;
using System.Net.Http;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json; // Cần cho Deserialization JSON
using System.Windows.Forms; // Cần cho MessageBox (cân nhắc loại bỏ trong lớp Helper)
using WordVaultAppMVC.Models; // Namespace chứa WordDetails model
using Newtonsoft.Json.Linq; // Cần cho JObject.Parse trong Translate
using System.Diagnostics;
using System.Collections.Generic; // Cần cho List<T>

namespace WordVaultAppMVC.Helpers
{
    /// <summary>
    /// Lớp tiện ích tĩnh để tương tác với các API từ điển và dịch thuật bên ngoài.
    /// </summary>
    public static class DictionaryApiClient
    {
        #region Private Static Fields

        // Sử dụng một instance HttpClient tĩnh để tái sử dụng kết nối và cải thiện hiệu suất.
        // Khuyến nghị của Microsoft: HttpClient được thiết kế để tái sử dụng.
        private static readonly HttpClient httpClient = new HttpClient()
        {
            // Đặt Timeout để tránh chờ đợi quá lâu nếu API không phản hồi.
            Timeout = TimeSpan.FromSeconds(30)
        };

        // URL các API
        private const string DictionaryApiUrlFormat = "https://api.dictionaryapi.dev/api/v2/entries/en/{0}"; // {0} sẽ thay bằng từ cần tra
        private const string TranslationApiUrlFormat = "https://api.mymemory.translated.net/get?q={0}&langpair=en|vi"; // {0} sẽ thay bằng text cần dịch

        #endregion

        #region Public Static Methods

        /// <summary>
        /// Lấy chi tiết thông tin của một từ tiếng Anh từ API dictionaryapi.dev.
        /// </summary>
        /// <param name="word">Từ tiếng Anh cần tra cứu.</param>
        /// <returns>Một đối tượng Task chứa WordDetails nếu tìm thấy, ngược lại trả về null.</returns>
        public static async Task<WordDetails> GetWordDetailsAsync(string word)
        {
            // Kiểm tra đầu vào.
            if (string.IsNullOrWhiteSpace(word))
            {
                Debug.WriteLine("[WARN] GetWordDetailsAsync: Từ cần tra là null hoặc rỗng.");
                return null;
            }

            // Chuẩn bị URL, trim và chuyển sang chữ thường.
            string apiUrl = string.Format(DictionaryApiUrlFormat, word.Trim().ToLower());
            Debug.WriteLine($"[INFO] Calling Dictionary API: {apiUrl}");

            try
            {
                // Gửi yêu cầu GET bất đồng bộ.
                HttpResponseMessage response = await httpClient.GetAsync(apiUrl);

                // Xử lý nếu API không trả về mã thành công (2xx).
                if (!response.IsSuccessStatusCode)
                {
                    // Trường hợp phổ biến: API trả về 404 Not Found nếu không tìm thấy từ.
                    if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        Debug.WriteLine($"[INFO] Word '{word}' not found on dictionaryapi.dev.");
                        // Cân nhắc không hiển thị MessageBox ở đây, để lớp gọi xử lý thông báo.
                        MessageBox.Show($"Không tìm thấy từ \"{word}\" trong từ điển.", "Không tìm thấy", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else // Các lỗi khác từ API (5xx, 4xx khác).
                    {
                        Debug.WriteLine($"[ERROR] Dictionary API request failed. Status: {(int)response.StatusCode} ({response.ReasonPhrase})");
                        MessageBox.Show($"API từ điển trả về lỗi: {(int)response.StatusCode} - {response.ReasonPhrase}");
                    }
                    return null; // Trả về null nếu API lỗi hoặc không tìm thấy.
                }

                // Đọc nội dung JSON từ phản hồi.
                string jsonString = await response.Content.ReadAsStringAsync();
                Debug.WriteLine("[INFO] --- dictionaryapi.dev Response JSON (Partial) ---");
                Debug.WriteLine(jsonString.Length > 500 ? jsonString.Substring(0, 500) + "..." : jsonString);
                Debug.WriteLine("[INFO] --- End dictionaryapi.dev Response JSON ---");

                // Deserialize JSON thành danh sách các kết quả (API này trả về mảng).
                var apiResponseList = JsonConvert.DeserializeObject<List<DictionaryApiResponse>>(jsonString);

                // Kiểm tra kết quả Deserialize.
                if (apiResponseList == null || !apiResponseList.Any())
                {
                    Debug.WriteLine("[WARN] GetWordDetailsAsync: JSON parsed as null or empty list.");
                    return null;
                }

                // Lấy kết quả đầu tiên trong danh sách.
                var firstEntry = apiResponseList.First();

                // --- Trích xuất thông tin từ đối tượng JSON đã parse ---
                // Lấy phiên âm đầu tiên có text.
                string pronunciation = firstEntry.phonetics?.FirstOrDefault(p => !string.IsNullOrEmpty(p.text))?.text ?? "";

                // Lấy audio URL: Ưu tiên file ...us.mp3 nếu có, nếu không lấy file audio đầu tiên tìm thấy.
                string audioUrl = firstEntry.phonetics?.FirstOrDefault(p => !string.IsNullOrEmpty(p.audio) && p.audio.EndsWith("us.mp3", StringComparison.OrdinalIgnoreCase))?.audio
                                 ?? firstEntry.phonetics?.FirstOrDefault(p => !string.IsNullOrEmpty(p.audio))?.audio ?? "";

                // Lấy định nghĩa đầu tiên của nghĩa đầu tiên.
                string meaning = firstEntry.meanings?.FirstOrDefault()?.definitions?.FirstOrDefault()?.definition ?? "Không tìm thấy định nghĩa.";

                // Tạo và trả về đối tượng WordDetails.
                return new WordDetails
                {
                    // Sử dụng từ gốc từ API nếu có, nếu không dùng từ người dùng nhập.
                    Word = firstEntry.word ?? word.Trim(),
                    Pronunciation = pronunciation,
                    AudioUrl = audioUrl,
                    Meaning = meaning
                    // Có thể thêm các trường khác nếu cần (PartOfSpeech, Examples...)
                };
            }
            catch (TaskCanceledException timeoutEx) // Bắt lỗi Timeout cụ thể
            {
                Debug.WriteLine($"[ERROR] Dictionary API request timed out for '{word}': {timeoutEx.Message}");
                MessageBox.Show("Yêu cầu tra từ điển quá thời gian chờ (timeout). Vui lòng kiểm tra kết nối mạng.", "Lỗi Mạng", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return null;
            }
            catch (HttpRequestException httpEx) // Bắt lỗi kết nối mạng hoặc HTTP
            {
                Debug.WriteLine($"[ERROR] HTTP request error for '{word}': {httpEx.Message}");
                MessageBox.Show("Lỗi mạng hoặc lỗi khi gọi API từ điển: " + httpEx.Message, "Lỗi Mạng", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            catch (JsonException jsonEx) // Bắt lỗi khi parse JSON
            {
                Debug.WriteLine($"[ERROR] JSON parsing error for '{word}': {jsonEx.Message}");
                MessageBox.Show("Lỗi xử lý dữ liệu trả về từ từ điển.", "Lỗi Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            catch (Exception ex) // Bắt các lỗi không mong muốn khác
            {
                Debug.WriteLine($"[ERROR] Unexpected error in GetWordDetailsAsync for '{word}': {ex.Message}");
                MessageBox.Show("Lỗi không xác định khi tra từ điển: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        /// <summary>
        /// Dịch một đoạn văn bản từ tiếng Anh sang tiếng Việt sử dụng API mymemory.translated.net.
        /// </summary>
        /// <param name="textToTranslate">Đoạn văn bản tiếng Anh cần dịch.</param>
        /// <returns>Một đối tượng Task chứa chuỗi tiếng Việt đã dịch, hoặc chuỗi gốc nếu dịch lỗi/không thành công.</returns>
        public static async Task<string> TranslateToVietnamese(string textToTranslate)
        {
            if (string.IsNullOrWhiteSpace(textToTranslate))
            {
                return string.Empty; // Trả về rỗng nếu không có gì để dịch
            }

            // Mặc định trả về text gốc nếu có lỗi.
            string translatedText = textToTranslate;
            // Tạo URL, mã hóa text cần dịch để tránh lỗi URL.
            string apiUrl = string.Format(TranslationApiUrlFormat, Uri.EscapeDataString(textToTranslate));
            Debug.WriteLine($"[INFO] Calling Translation API: {apiUrl.Substring(0, Math.Min(apiUrl.Length, 100))}..."); // Log ngắn gọn

            try
            {
                // Sử dụng một HttpClient tạm thời cho API này nếu cần timeout khác.
                // Hoặc có thể dùng chung httpClient tĩnh nếu timeout phù hợp.
                using (var translationClient = new HttpClient { Timeout = TimeSpan.FromSeconds(15) })
                {
                    // Gửi yêu cầu GET và đọc phản hồi dạng chuỗi.
                    var responseString = await translationClient.GetStringAsync(apiUrl);
                    // Parse JSON bằng Newtonsoft.Json.Linq.JObject.
                    var jsonResponse = JObject.Parse(responseString);

                    // Trích xuất trường "translatedText" từ "responseData".
                    string apiTranslated = jsonResponse["responseData"]?["translatedText"]?.ToString();

                    // Chỉ cập nhật nếu bản dịch khác bản gốc và không rỗng.
                    if (!string.IsNullOrEmpty(apiTranslated) && !apiTranslated.Equals(textToTranslate, StringComparison.OrdinalIgnoreCase))
                    {
                        translatedText = apiTranslated;
                        Debug.WriteLine($"[INFO] Translation successful: '{textToTranslate}' -> '{translatedText}'");
                    }
                    else
                    {
                        Debug.WriteLine($"[WARN] TranslateToVietnamese: Translation same as original or empty/null for '{textToTranslate}'. API response: {apiTranslated}");
                    }
                }
            }
            catch (TaskCanceledException timeoutEx)
            {
                Debug.WriteLine($"[ERROR] Translation API request timed out for '{textToTranslate}': {timeoutEx.Message}");
                // Không hiện MessageBox, chỉ trả về text gốc
            }
            catch (HttpRequestException httpEx)
            {
                Debug.WriteLine($"[ERROR] Translation HTTP request error for '{textToTranslate}': {httpEx.Message}");
                // Không hiện MessageBox
            }
            catch (JsonException jsonEx)
            {
                Debug.WriteLine($"[ERROR] Translation JSON parsing error for '{textToTranslate}': {jsonEx.Message}");
                // Không hiện MessageBox
            }
            catch (Exception ex) // Bắt các lỗi khác
            {
                Debug.WriteLine($"[ERROR] Unexpected error in TranslateToVietnamese for '{textToTranslate}': {ex.Message}");
                // Không hiện MessageBox
            }

            // Luôn trả về một chuỗi (bản gốc hoặc bản dịch).
            return translatedText;
        }

        #endregion

        #region Private Nested JSON Models for dictionaryapi.dev

        // Các lớp này được định nghĩa là private và chỉ dùng nội bộ trong DictionaryApiClient
        // để giúp Newtonsoft.Json deserialize phản hồi từ API dictionaryapi.dev.
        // Tên thuộc tính phải khớp với tên trường trong JSON (phân biệt chữ hoa/thường).

        /// <summary>Mô hình hóa cấu trúc cấp cao nhất của phản hồi JSON (là một mảng).</summary>
        private class DictionaryApiResponse
        {
            public string word { get; set; }
            public List<Phonetic> phonetics { get; set; }
            public List<Meaning> meanings { get; set; }
            // Các trường khác có thể có: license, sourceUrls
        }

        /// <summary>Mô hình hóa thông tin phiên âm.</summary>
        private class Phonetic
        {
            public string text { get; set; } // Phiên âm dạng text
            public string audio { get; set; } // URL file âm thanh
            // Các trường khác có thể có: sourceUrl, license
        }

        /// <summary>Mô hình hóa một nghĩa của từ (theo loại từ).</summary>
        private class Meaning
        {
            public string partOfSpeech { get; set; } // Loại từ (noun, verb,...)
            public List<Definition> definitions { get; set; } // Danh sách các định nghĩa
            public List<string> synonyms { get; set; } // Từ đồng nghĩa
            public List<string> antonyms { get; set; } // Từ trái nghĩa
        }

        /// <summary>Mô hình hóa một định nghĩa cụ thể của một nghĩa.</summary>
        private class Definition
        {
            public string definition { get; set; } // Nội dung định nghĩa
            public List<string> synonyms { get; set; } // Từ đồng nghĩa riêng cho định nghĩa này
            public List<string> antonyms { get; set; } // Từ trái nghĩa riêng
            public string example { get; set; } // Ví dụ sử dụng
        }

        #endregion
    }
}