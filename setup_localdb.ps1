# Script para criar o banco de dados KanbanDB no LocalDB
Write-Host "Configurando LocalDB..." -ForegroundColor Cyan

# Verificar se LocalDB esta rodando
$info = SqlLocalDB info MSSQLLocalDB
if ($info -match "Executando") {
    Write-Host "LocalDB esta executando" -ForegroundColor Green
} else {
    Write-Host "Iniciando LocalDB..." -ForegroundColor Yellow
    SqlLocalDB start MSSQLLocalDB
}

# Ler o script SQL
$sqlScript = Get-Content "migration_sqlserver.sql" -Raw

# Conectar e executar
Write-Host "Criando banco de dados e tabelas..." -ForegroundColor Cyan

try {
    # Carregar assembly SQL (usar System.Data.SqlClient que ja vem no Windows)
    Add-Type -AssemblyName "System.Data"
    
    # Connection string
    $connectionString = "Server=(localdb)\MSSQLLocalDB;Integrated Security=true;TrustServerCertificate=True;"
    
    # Conectar
    $connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
    $connection.Open()
    
    # Dividir script em batches (separados por GO)
    $batches = $sqlScript -split '\bGO\b'
    
    foreach ($batch in $batches) {
        $batch = $batch.Trim()
        if ($batch -ne "") {
            $command = New-Object System.Data.SqlClient.SqlCommand($batch, $connection)
            $command.CommandTimeout = 60
            try {
                $command.ExecuteNonQuery() | Out-Null
                Write-Host "Batch executado com sucesso" -ForegroundColor Green
            } catch {
                Write-Host "Aviso: $($_.Exception.Message)" -ForegroundColor Yellow
            }
        }
    }
    
    $connection.Close()
    
    Write-Host ""
    Write-Host "BANCO DE DADOS CRIADO COM SUCESSO!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Verificando tabelas criadas..." -ForegroundColor Cyan
    
    # Verificar tabelas
    $connection = New-Object System.Data.SqlClient.SqlConnection("Server=(localdb)\MSSQLLocalDB;Database=KanbanDB;Integrated Security=true;TrustServerCertificate=True;")
    $connection.Open()
    
    $command = New-Object System.Data.SqlClient.SqlCommand("SELECT name FROM sys.tables ORDER BY name", $connection)
    $reader = $command.ExecuteReader()
    
    Write-Host ""
    Write-Host "Tabelas criadas:" -ForegroundColor White
    while ($reader.Read()) {
        Write-Host "  $($reader["name"])" -ForegroundColor Cyan
    }
    
    $reader.Close()
    $connection.Close()
    
    Write-Host ""
    Write-Host "TUDO PRONTO!" -ForegroundColor Green
    Write-Host ""
    Write-Host "Connection String configurada em appsettings.json:" -ForegroundColor Yellow
    Write-Host 'Server=(localdb)\MSSQLLocalDB;Database=KanbanDB;Integrated Security=true;TrustServerCertificate=True;' -ForegroundColor White
    Write-Host ""
    Write-Host "Proximo passo: Execute 'dotnet run' na pasta KanbanAPI" -ForegroundColor Cyan
    
} catch {
    Write-Host ""
    Write-Host "ERRO: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "Stack trace:" -ForegroundColor Yellow
    Write-Host $_.Exception.StackTrace -ForegroundColor Gray
    exit 1
}
