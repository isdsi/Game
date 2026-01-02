# [1단계: 빌드] - 작업자가 작성한 코드를 리눅스용 단일 파일로 굽습니다.
# 빌드 타겟 아키텍처에 맞춰 수행합니다.
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# 도커 빌드 시 자동으로 들어오는 기본 변수들입니다.
ARG TARGETARCH
ARG TARGETOS

COPY . .
# 빌드 타겟 아키텍처에 맞춰 dotnet publish를 수행합니다.
RUN dotnet publish -c Release -r $TARGETOS-$TARGETARCH --self-contained true \
    -p:PublishSingleFile=true -p:PublishTrimmed=true -o out

# [2단계: 실행] - 가장 가벼운 리눅스 환경(추상화)에서 실행합니다.
FROM mcr.microsoft.com/dotnet/runtime-deps:8.0-jammy
WORKDIR /app
COPY --from=build /app/out .

# 서버가 사용할 포트 명시 (도커의 울타리)
EXPOSE 5000

# 서비스 등록 없이, 일반 실행 파일로 실행 (도커가 관리)
ENTRYPOINT ["./GameServer"]
