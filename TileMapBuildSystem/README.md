# TileMapBuildSystem

TileMapBuildSystem은 Unity 프로젝트에서 타일맵 기반 건설 시스템을 구현하기 위한 유틸리티/모듈입니다.

Unity의 타일맵 시스템을 활용해 건물 배치, 충돌 처리, NPC와 건물간 상호작용 등을 구조화하는 데 목적이 있습니다.

해당 폴더는 2D 건설 경영 시뮬레이션 프로토타입 프로젝트의 코드 중 일부만 발췌해 커밋했습니다.

해당 코드만으로는 프로젝트가 정상적으로 작동하지 않을 수 있음을 알려드립니다.


## 대표 아키텍쳐 요약

	### GridBuildSystem.cs
	    싱글톤패턴으로, 타일맵(Main, Temp, Place) 관리,건물 설치와 설치된 건물 등록 등을 담당합니다.
	### GridBuilding.cs
		모든 건물의 기초가 되는 컴포넌트로, 설치 상태,건물 상태 수정 모드 UI, 영역 감지 로직을 포함합니다.
	### BuildingManager.cs
		건물에 부착되는 ScriptableObject로, NPC 고용, 인벤토리, NPC에게 작업(Task) 할당 등을 처리합니다.
	### BuildingFunction.cs
		건물과 NPC간의 상호작용 로직을 위한 추상 계층입니다.

