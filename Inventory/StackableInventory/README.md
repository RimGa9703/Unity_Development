# Stackable Inventory

	스택 가능한 아이템 인벤토리 모듈이 있는 폴더 입니다.
	
	아이템 코드 기반으로 여러 스택을 관리하며, 최대 스택 수 / 인벤토리 슬롯 제한 / 아이템 개수 이동(inventory to inventory)까지 구현되어 있습니다.
	
## 대표 아키텍쳐 요약

	### GameData.cs
	데이터 계층의 베이스 클래스 입니다.

	### GameItem.cs
	실제 게임 아이템 클래스로 게임 아이템의 데이터를 담당힙니다.

	### StackableInventory<T>.cs
	스택형 아이템을 저장하는 인벤토리의 핵심 로직을 담당합니다.

	### GameItemInventory.cs
	StackableInventory을 상속 받는 실제 게임 아이템 데이터를 담는 역할을 담당합니다.


