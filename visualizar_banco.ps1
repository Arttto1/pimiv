# Script para visualizar tabelas do LocalDB
Write-Host "==============================================";
Write-Host "  VISUALIZADOR DE TABELAS - LocalDB KanbanDB" -ForegroundColor Cyan
Write-Host "==============================================`n";

# Connection string
$connectionString = "Server=(localdb)\MSSQLLocalDB;Database=KanbanDB;Integrated Security=true;TrustServerCertificate=True;"

try {
    # Conectar
    Add-Type -AssemblyName "System.Data"
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    
    Write-Host "✅ Conectado ao banco KanbanDB`n" -ForegroundColor Green
    
    while ($true) {
        Write-Host "`n=================== MENU ===================" -ForegroundColor Yellow
        Write-Host "1. Ver todas as tabelas"
        Write-Host "2. Ver usuários (pim_users)"
        Write-Host "3. Ver colunas (pim_columns)"
        Write-Host "4. Ver cards (pim_cards)"
        Write-Host "5. Ver tickets (pim_tickets)"
        Write-Host "6. Executar query personalizada"
        Write-Host "7. Criar novo usuário ADMIN"
        Write-Host "0. Sair"
        Write-Host "============================================`n" -ForegroundColor Yellow
        
        $opcao = Read-Host "Escolha uma opção"
        
        switch ($opcao) {
            "1" {
                Write-Host "`n📊 TABELAS DO BANCO:" -ForegroundColor Cyan
                $cmd = New-Object System.Data.SqlClient.SqlCommand("SELECT name, create_date FROM sys.tables ORDER BY name", $connection)
                $reader = $cmd.ExecuteReader()
                
                Write-Host ("{0,-30} {1}" -f "Nome da Tabela", "Data de Criação") -ForegroundColor White
                Write-Host ("-" * 60) -ForegroundColor Gray
                
                while ($reader.Read()) {
                    Write-Host ("{0,-30} {1}" -f $reader["name"], $reader["create_date"])
                }
                $reader.Close()
            }
            
            "2" {
                Write-Host "`n👥 USUÁRIOS (pim_users):" -ForegroundColor Cyan
                $cmd = New-Object System.Data.SqlClient.SqlCommand("SELECT id, username, admin, created_at FROM pim_users ORDER BY created_at DESC", $connection)
                $reader = $cmd.ExecuteReader()
                
                Write-Host ("{0,-38} {1,-20} {2,-10} {3}" -f "ID", "Username", "Admin?", "Criado em") -ForegroundColor White
                Write-Host ("-" * 100) -ForegroundColor Gray
                
                while ($reader.Read()) {
                    $isAdmin = if ($reader["admin"]) { "✅ SIM" } else { "❌ NÃO" }
                    Write-Host ("{0,-38} {1,-20} {2,-10} {3}" -f $reader["id"], $reader["username"], $isAdmin, $reader["created_at"])
                }
                $reader.Close()
            }
            
            "3" {
                Write-Host "`n📋 COLUNAS (pim_columns):" -ForegroundColor Cyan
                $cmd = New-Object System.Data.SqlClient.SqlCommand(@"
                    SELECT c.id, c.name, c.color, c.position, c.is_deletable, u.username as owner
                    FROM pim_columns c
                    JOIN pim_users u ON c.user_id = u.id
                    ORDER BY u.username, c.position
"@, $connection)
                $reader = $cmd.ExecuteReader()
                
                Write-Host ("{0,-38} {1,-20} {2,-10} {3,-5} {4,-12} {5}" -f "ID", "Nome", "Cor", "Pos", "Deletável?", "Dono") -ForegroundColor White
                Write-Host ("-" * 120) -ForegroundColor Gray
                
                while ($reader.Read()) {
                    $deletable = if ($reader["is_deletable"]) { "✅ SIM" } else { "🔒 NÃO" }
                    Write-Host ("{0,-38} {1,-20} {2,-10} {3,-5} {4,-12} {5}" -f $reader["id"], $reader["name"], $reader["color"], $reader["position"], $deletable, $reader["owner"])
                }
                $reader.Close()
            }
            
            "4" {
                Write-Host "`n🗂️  CARDS (pim_cards):" -ForegroundColor Cyan
                $cmd = New-Object System.Data.SqlClient.SqlCommand(@"
                    SELECT c.id, c.title, c.description, col.name as column_name, c.position, c.created_at
                    FROM pim_cards c
                    JOIN pim_columns col ON c.column_id = col.id
                    ORDER BY col.name, c.position
"@, $connection)
                $reader = $cmd.ExecuteReader()
                
                Write-Host ("{0,-38} {1,-30} {2,-20} {3,-5} {4}" -f "ID", "Título", "Coluna", "Pos", "Criado") -ForegroundColor White
                Write-Host ("-" * 140) -ForegroundColor Gray
                
                $count = 0
                while ($reader.Read()) {
                    $title = if ($reader["title"].Length -gt 30) { $reader["title"].Substring(0, 27) + "..." } else { $reader["title"] }
                    Write-Host ("{0,-38} {1,-30} {2,-20} {3,-5} {4}" -f $reader["id"], $title, $reader["column_name"], $reader["position"], $reader["created_at"])
                    $count++
                }
                $reader.Close()
                Write-Host "`nTotal: $count cards" -ForegroundColor Gray
            }
            
            "5" {
                Write-Host "`n🎫 TICKETS (pim_tickets):" -ForegroundColor Cyan
                $cmd = New-Object System.Data.SqlClient.SqlCommand(@"
                    SELECT t.id, t.title, u1.username as requester, u2.username as admin, c.title as card_title, t.created_at
                    FROM pim_tickets t
                    JOIN pim_users u1 ON t.requester_id = u1.id
                    JOIN pim_users u2 ON t.admin_id = u2.id
                    JOIN pim_cards c ON t.card_id = c.id
                    ORDER BY t.created_at DESC
"@, $connection)
                $reader = $cmd.ExecuteReader()
                
                Write-Host ("{0,-38} {1,-30} {2,-15} {3,-15} {4}" -f "ID", "Título", "Solicitante", "Admin", "Criado") -ForegroundColor White
                Write-Host ("-" * 140) -ForegroundColor Gray
                
                $count = 0
                while ($reader.Read()) {
                    $title = if ($reader["title"].Length -gt 30) { $reader["title"].Substring(0, 27) + "..." } else { $reader["title"] }
                    Write-Host ("{0,-38} {1,-30} {2,-15} {3,-15} {4}" -f $reader["id"], $title, $reader["requester"], $reader["admin"], $reader["created_at"])
                    $count++
                }
                $reader.Close()
                
                if ($count -eq 0) {
                    Write-Host "`n📭 Nenhum ticket criado ainda" -ForegroundColor Yellow
                } else {
                    Write-Host "`nTotal: $count tickets" -ForegroundColor Gray
                }
            }
            
            "6" {
                Write-Host "`n💻 EXECUTAR QUERY PERSONALIZADA:" -ForegroundColor Cyan
                Write-Host "Digite sua query SQL (exemplo: SELECT * FROM pim_users):" -ForegroundColor Yellow
                $query = Read-Host
                
                try {
                    $cmd = New-Object System.Data.SqlClient.SqlCommand($query, $connection)
                    
                    if ($query.Trim().ToUpper().StartsWith("SELECT")) {
                        $reader = $cmd.ExecuteReader()
                        
                        # Mostrar colunas
                        Write-Host "`nResultados:" -ForegroundColor Green
                        $columns = @()
                        for ($i = 0; $i -lt $reader.FieldCount; $i++) {
                            $columns += $reader.GetName($i)
                        }
                        Write-Host ($columns -join " | ") -ForegroundColor White
                        Write-Host ("-" * 120) -ForegroundColor Gray
                        
                        # Mostrar dados
                        $count = 0
                        while ($reader.Read()) {
                            $row = @()
                            for ($i = 0; $i -lt $reader.FieldCount; $i++) {
                                $row += $reader[$i]
                            }
                            Write-Host ($row -join " | ")
                            $count++
                        }
                        $reader.Close()
                        Write-Host "`nTotal: $count linhas" -ForegroundColor Gray
                    } else {
                        $rowsAffected = $cmd.ExecuteNonQuery()
                        Write-Host "`n✅ Query executada! $rowsAffected linha(s) afetada(s)" -ForegroundColor Green
                    }
                } catch {
                    Write-Host "`n❌ ERRO: $($_.Exception.Message)" -ForegroundColor Red
                }
            }
            
            "7" {
                Write-Host "`n👑 CRIAR NOVO USUÁRIO ADMIN:" -ForegroundColor Cyan
                $username = Read-Host "Username"
                $password = Read-Host "Senha" -AsSecureString
                $passwordPlain = [Runtime.InteropServices.Marshal]::PtrToStringAuto([Runtime.InteropServices.Marshal]::SecureStringToBSTR($password))
                
                # Gerar hash BCrypt (simplificado - na real usa BCrypt.Net)
                Write-Host "`n⚠️  ATENÇÃO: Você precisa gerar o hash BCrypt da senha manualmente" -ForegroundColor Yellow
                Write-Host "Use o endpoint /api/auth/register da API para criar o usuário com hash correto" -ForegroundColor Yellow
                Write-Host "`nOU execute este SQL no Azure Data Studio/SSMS:" -ForegroundColor Cyan
                Write-Host "UPDATE pim_users SET admin = 1 WHERE username = '$username';" -ForegroundColor White
            }
            
            "0" {
                Write-Host "`n👋 Saindo..." -ForegroundColor Yellow
                $connection.Close()
                return
            }
            
            default {
                Write-Host "`n❌ Opção inválida!" -ForegroundColor Red
            }
        }
        
        Write-Host "`nPressione Enter para continuar..."
        $null = Read-Host
    }
    
} catch {
    Write-Host "`n❌ ERRO: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host $_.Exception.StackTrace -ForegroundColor Gray
} finally {
    if ($connection -and $connection.State -eq 'Open') {
        $connection.Close()
    }
}
