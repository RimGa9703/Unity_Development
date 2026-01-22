
# Projectile
발사체 로직 모음 폴더

## CurvedGuideMissile.cs
    끊임없이 곡선 추격하는 발사체 로직 
    ps.메이플스토리 아델-크리에이션 스킬 참조
![Image](https://github.com/user-attachments/assets/88499469-0c57-4f07-8120-97c10b103e48)
    
### 사용법
    발사체가 되는 오브젝트에 해당 스크립트 부착 후 타겟으로 할 오브젝트를 인스펙터에서 링크걸어주면 됩니다.
### 변수 설명
    targetTf = 발사체의 타겟
    rotationWeight = 발사체의 선회력 값이 클수록 더 크게 선회한다.
    accelerationWeight = 발사체가 물체로 향할 때의 가속도 값이 클수록 빨라짐
    maxSpeed = accelerationWeight로 인해 속도가 한없이 커지는것을 방지 하기 위한 변수
    minSpeed = accelerationWeight로 인해 속도가 한없이 작아지는것을 방지 하기 위한 변수

## RotateAroundMissile.cs
    타겟 주의를 멤도는 발사체 로직
    ps.흔히 뱀서라이크에서 사용하는 내 주위를 멤돌며 타겟을 보호하는 발사체
### 사용법
    발사체가 되는 오브젝트에 해당 스크립트 부착 후 타겟으로 할 오브젝트를 인스펙터에서 링크걸어주면 됩니다.
### 변수 설명
    target = 멤돌 대상
    prefab = 발사체 오브젝트
    targetToMissileDist = 멤돌 대상과의 거리
    rotateSpeed = 회전 속도
    HoverAroundMissileCount = 생성할 미사일 개수
    
    
    
    

    

    
    
        
