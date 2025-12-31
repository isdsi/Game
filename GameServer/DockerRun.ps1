$Image = "game-server-docker-image"
$Tag = "latest"
$ContainerName = "game-server-docker-name"

# 컨테이너 중단
docker stop $ContainerName

# 컨테이너 삭제
docker rm $ContainerName

# 이미지 삭제
#docker rmi $id
docker rmi $Image

# 빌드해서 이미지 생성
docker build -o type=docker,dest="${Image}.tar" .

# 이미지를 도커 리포지터리로 불러오기
docker load -i "${Image}.tar"

# 이미지ID 얻어내기
$id = (docker load -i"${Image}.tar" | Select-String -Pattern "sha256:([a-f0-9]+)" | ForEach-Object { $_.Matches.Groups[1].Value }); 

# 이미지에 태그를 붙여주기. 이미지 번호가 매번 달라진다.
docker tag $id "${Image}:${Tag}"

# 컨테이너 생성, DB의 IP를 컨테이너 전체를 바라보게 한다.
docker create --name $ContainerName -p 5000:5000 -e DB_HOST=host.docker.internal "${Image}:${Tag}"

# 컨테이너 생성 및 시작
# docker run -d --name $ContainerName -p 5000:5000 "${Image}:${Tag}"

# 컨테이너 시작
docker start $ContainerName

# 컨테이어 로그 보기
docker logs -f $ContainerName
