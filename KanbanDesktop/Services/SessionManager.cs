using System;
using System.IO;
using Newtonsoft.Json;

namespace KanbanDesktop.Services;

public class SessionManager
{
    private static readonly string SessionFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "KanbanApp",
        "session.json"
    );

    public class SessionData
    {
        public Guid UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    public static void SaveSession(Guid userId, string username, bool isAdmin)
    {
        try
        {
            var directory = Path.GetDirectoryName(SessionFilePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory!);
            }

            var session = new SessionData
            {
                UserId = userId,
                Username = username,
                IsAdmin = isAdmin,
                ExpiresAt = DateTime.Now.AddDays(1)
            };

            var json = JsonConvert.SerializeObject(session);
            File.WriteAllText(SessionFilePath, json);

            Console.WriteLine($"[SESSION] ✅ Sessão salva: {username} (Admin: {isAdmin}) (expira em 1 dia)");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SESSION] ❌ Erro ao salvar sessão: {ex.Message}");
        }
    }

    public static SessionData? LoadSession()
    {
        try
        {
            if (!File.Exists(SessionFilePath))
            {
                Console.WriteLine($"[SESSION] ℹ️ Nenhuma sessão salva encontrada");
                return null;
            }

            var json = File.ReadAllText(SessionFilePath);
            var session = JsonConvert.DeserializeObject<SessionData>(json);

            if (session == null)
            {
                Console.WriteLine($"[SESSION] ⚠️ Sessão inválida");
                return null;
            }

            if (session.ExpiresAt < DateTime.Now)
            {
                Console.WriteLine($"[SESSION] ⏰ Sessão expirada em {session.ExpiresAt}");
                ClearSession();
                return null;
            }

            Console.WriteLine($"[SESSION] ✅ Sessão válida: {session.Username}");
            return session;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SESSION] ❌ Erro ao carregar sessão: {ex.Message}");
            return null;
        }
    }

    public static void ClearSession()
    {
        try
        {
            if (File.Exists(SessionFilePath))
            {
                File.Delete(SessionFilePath);
                Console.WriteLine($"[SESSION] 🗑️ Sessão removida");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SESSION] ❌ Erro ao limpar sessão: {ex.Message}");
        }
    }
}
