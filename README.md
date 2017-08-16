# Statistics
Stockholm프로젝트 코드의 일부입니다. **캐릭터의 능력치와 버프,디버프 등을 관리**하기 위해 사용합니다.

## 요약
1. StatisticsManager은 스탯의 기본값을 지정하고, 연산을 수행하는 컴포넌트입니다.
2. StatisticsExpression은 스탯을 변경하는 계산식을 담는 컴포넌트입니다.
3. IStatisticsValue를 구현한 클래스는 스탯 값을 저장하는 대상이 됩니다.

## 코드상에서 사용하는 방법
```
[SerializeField]
private StatisticsManager manager_; // 컴포넌트
public Statistics<PlayerValue> statistics;

// Statistics객체를 초기화하려면 이렇게 합니다.
statistics = manager_.GetStatisticsInstance<PlayerValue>();

// Statistics객체에 담긴 정보는 이렇게 사용합니다.
Debug.LogFormat("기본값 maxHp: {0}",statistics.defaultStat.maxHp);
Debug.LogFormat("현재 maxHp: {0}", statistics.currentStat.maxHp);
```


## Statistics를 사용하는 객체를 외부에서 사용하는 방법
```
public Player player;

// 외부에서 객체가 가진 스탯에 접근하는 방법
Debug.LogFormat("기본값 maxHp: {0}",player.statistics.defaultStat.maxHp);
Debug.LogFormat("현재 maxHp: {0}", player.statistics.currentStat.maxHp);

// 계산식을 적용하는 방법
public StatisticsExpression expression; // 계산식 컴포넌트입니다

player.statsitics.AddExpression(expression);
player.statsitics.RemoveExpression(expression);
player.statsitics.ForEachExpression(exp => ..);
player.statistics.FindExpression(exp => ..);
```

## 상속을 통한 값 관리

1. StatisticsValue는 상속을 통해 값 관리가 가능합니다.
2. 예를 들어, PlayerValue가 CreatureValue를 상속받는다면, CreatureValue 계산식을 PlayerValue에 적용할 수 있습니다. 
2. 또한, 값이 PlayerValue로 지정된 StatisticsManager 컴포넌트에서 GetStatisticsInstance\<**CreatureValue**\>() 식으로 가져올 수도 있습니다.

## 샘플 소개

- 주황색 상자를 방향키로 움직이고 스페이스로 점프할 수 있습니다.
- 초록색 발판 위에 올라서면 이동속도가 느려집니다.
- 파란색 발판 위에 올라서면 더 높게 점프할수 있습니다.
- 에디터에서 플레이중일 경우, StatisticsManager 컴포넌트에서 실시간으로 값과 적용된 Expression목록을 확인할 수 있습니다.