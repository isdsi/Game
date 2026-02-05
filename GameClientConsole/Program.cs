using Serilog;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;
using GameClientConsole;
using GameClientPoco;

// 1. Serilog 설정 (여기서는 전역 정적 클래스 Log를 사용하므로 ILogger와 충돌 안 함)
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/game.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

// 2. LoggerFactory 생성 (Microsoft 소속임을 명시)
using ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddSerilog(); // Serilog를 구현체로 주입
});

// 3. 로거 생성 (Microsoft.Extensions.Logging.ILogger 타입)
Microsoft.Extensions.Logging.ILogger logger = loggerFactory.CreateLogger("GameClient");
logger.LogInformation("Hello, GameClient");

Game _game = new Game(logger);
_game.Play();

// 프로그램 종료 시 로그 버퍼 비우기
Log.CloseAndFlush();
