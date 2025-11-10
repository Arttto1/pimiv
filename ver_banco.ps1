# Visualizador Simples de Banco LocalDB
Write-Host "=============================================="
Write-Host "  VISUALIZADOR LocalDB - KanbanDB" -ForegroundColor Cyan
Write-Host "==============================================`n"

$connectionString = "Server=(localdb)\MSSQLLocalDB;Database=KanbanDB;Integrated Security=true;TrustServerCertificate=True;"

function Show-Query {
    param($query, $title)
    
    Write-Host "`n$title" -ForegroundColor Cyan
    Write-Host ("-" * 80) -ForegroundColor Gray
    
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    
    $cmd = New-Object System.Data.SqlClient.SqlCommand($query, $connection)
    $reader = $cmd.ExecuteReader()
    
    $count = 0
    while ($reader.Read()) {
        $line = ""
        for ($i = 0; $i -lt $reader.FieldCount; $i++) {
            $line += "$($reader.GetName($i)): $($reader[$i])  "
        }
        Write-Host $line
        $count++
    }
    
    $reader.Close()
    $connection.Close()
    
    Write-Host "`nTotal: $count registros" -ForegroundColor Gray
}

# Menu
while ($true) {
    Write-Host "`n=================== MENU ===================" -ForegroundColor Yellow
    Write-Host "1. Ver usuarios"
    Write-Host "2. Ver colunas"
    Write-Host "3. Ver cards"
    Write-Host "4. Ver tickets"
    Write-Host "5. Tornar usuario ADMIN"
    Write-Host "0. Sair"
    Write-Host "============================================`n" -ForegroundColor Yellow
    
    $opcao = Read-Host "Escolha"
    
    if ($opcao -eq "1") {
        Show-Query "SELECT id, username, admin, created_at FROM pim_users ORDER BY created_at DESC" "USUARIOS"
    }
    elseif ($opcao -eq "2") {
        Show-Query "SELECT c.id, c.name, c.color, c.is_deletable, u.username as owner FROM pim_columns c JOIN pim_users u ON c.user_id = u.id ORDER BY u.username" "COLUNAS"
    }
    elseif ($opcao -eq "3") {
        Show-Query "SELECT c.id, c.title, col.name as coluna, c.position FROM pim_cards c JOIN pim_columns col ON c.column_id = col.id ORDER BY col.name" "CARDS"
    }
    elseif ($opcao -eq "4") {
        Show-Query "SELECT t.id, t.title, u1.username as solicitante, u2.username as admin FROM pim_tickets t JOIN pim_users u1 ON t.requester_id = u1.id JOIN pim_users u2 ON t.admin_id = u2.id" "TICKETS"
    }
    elseif ($opcao -eq "5") {
        $username = Read-Host "Username do usuario"
        $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
        $connection.Open()
        $cmd = New-Object System.Data.SqlClient.SqlCommand("UPDATE pim_users SET admin = 1 WHERE username = @username", $connection)
        $cmd.Parameters.AddWithValue("@username", $username)
        $rows = $cmd.ExecuteNonQuery()
        $connection.Close()
        
        if ($rows -gt 0) {
            Write-Host "`nUsuario '$username' agora e ADMIN!" -ForegroundColor Green
        } else {
            Write-Host "`nUsuario nao encontrado!" -ForegroundColor Red
        }
    }
    elseif ($opcao -eq "0") {
        Write-Host "`nSaindo..." -ForegroundColor Yellow
        break
    }
    else {
        Write-Host "`nOpcao invalida!" -ForegroundColor Red
    }
    
    Write-Host "`nPressione Enter..."
    $null = Read-Host
}
