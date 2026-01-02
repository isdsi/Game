$Image = "game-server-docker-image"
$Tag = "latest"
$ContainerName = "game-server-docker-name"
$Platform = "linux/arm64"
$FileName = "${Image}-all-platforms.tar"
$ImageTag = "${Image}:${Tag}"

# 컨테이너 중단
docker stop $ContainerName

# 컨테이너 삭제
docker rm $ContainerName

# 이미지 삭제
#docker rmi $id
docker rmi $Image

# 이미지를 사용하는 모든 컨테이너 삭제
# -q는 ID만 출력, -a는 전체, --filter는 특정 이미지 기반 컨테이너만 골라냅니다.
#docker rm -f $(docker ps -aq --filter ancestor=$Image)

# 빌드해서 통합 이미지 생성
docker buildx build --platform linux/amd64,linux/arm64 `
    -o type=oci,dest=$FileName .

# 이미지의 index.json 파일 압축해제
#tar -xf $FileName index.json -O 

# 이미지를 도커 리포지터리로 불러오기
docker load -i $FileName

# 이미지ID 얻어내기
$id = (docker load -i $FileName | Select-String -Pattern "sha256:([a-f0-9]+)" | ForEach-Object { $_.Matches.Groups[1].Value }); 

# 이미지에 태그를 붙여주기. 이미지 번호가 매번 달라진다.
docker tag $id $ImageTag

# 이미지의 플랫폼 확인하기
docker inspect $ImageTag

# 컨테이너 생성, DB의 IP를 컨테이너 전체를 바라보게 한다. 
# 윈도우
docker create --name $ContainerName `
    -p 5000:5000 `
    -e DB_HOST=host.docker.internal $ImageTag

# 리눅스
docker create --name $ContainerName `
    --add-host=host.docker.internal:host-gateway `
    -p 5000:5000 `
    -e DB_HOST=host.docker.internal $ImageTag
  
# 컨테이너 생성 및 시작
# docker run -d --name $ContainerName -p 5000:5000 $ImageTag

# 컨테이너 시작
docker start $ContainerName

# 컨테이어 로그 보기
docker logs -f $ContainerName
#docker logs $ContainerName # tail 안함
