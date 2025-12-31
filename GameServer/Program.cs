using Microsoft.Extensions.Logging;

string connectionString = "Host=localhost;Username=admin;Password=password123;Database=game_server_db";

ILoggerFactory _loggerFactory;
ILogger _logger;

_loggerFactory = LoggerFactory.Create(builder => builder.AddConsole());
_logger = _loggerFactory.CreateLogger("GameServer");
IGameRepository _gameRepository = new PostgreSqlGameRepository(connectionString, _logger);

GameServer gameServer = new GameServer(_gameRepository, _logger, 5000);

gameServer.Process();