$Image = "game-server-docker-image"
$FileName = "${Image}-all-platforms.tar"

# 형식: scp [보낼파일경로] [파이계정]@[파이IP]:[저장할경로]
$PiUser = "cat"          # 라즈베리 파이 사용자 이름 (기본값 pi)
$PiIP = "isdsi"  # 대표님의 라즈베리 파이 IP 주소

scp $FileName ${PiUser}@${PiIP}:/home/$PiUser/