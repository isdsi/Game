# D 드라이브에 데이터 저장 폴더 생성
New-Item -Path "D:\DockerData\postgres" -ItemType Directory -Force

# PostgreSQL 컨테이너 실행
# 수정된 실행 명령어 (도커 내부 경로를 /var/lib/postgresql 로 변경)
docker run -d `
  --name game-server-db `
  -e POSTGRES_USER=admin `
  -e POSTGRES_PASSWORD=password123 `
  -e POSTGRES_DB=game_server_db `
  -v D:\DockerData\postgres:/var/lib/postgresql `
  -p 5432:5432 `
  postgres:18
  
