# MyGameplayEffectSystem

언리얼엔진의 GAS에서 GameplayEffect 부분만을 떼어낸 느낌으로 만든 시스템


이펙트가 부여되면 즉시, 지속, 영구 형태로 적용시키고, 이펙트가 끝나면 제거됨.

GameplayEfectSystem 컴포넌트와 GameplayEffect 스크립터블 오브젝트로 이루어져있음.

GameplayEffectSystem 에 GameplayEffect를 부여하면, GameplayEffect에서 IEffectSpec을 생성하여 부여한다.

부여한 다음에 이펙트 발동 가능 여부에 따라 효과를 처리한다.


https://github.com/kimjisoo4/MyUtilities 가 필요함.

- 사용은 자유이나 그로 인해 생긴 오류에 대해서는 책임지지 않음.

자세한 정보 : --
