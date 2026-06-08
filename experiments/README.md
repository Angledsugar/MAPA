# Experiments

설계 근거: [`../docs/research-design.md`](../docs/research-design.md) §7–9.

## 2×2 factorial (init × transfer-mode)

|  | Zero-shot | Retrain (clean shared reward) |
|---|---|---|
| **Clean-init** | ① 전이 baseline | ③ co-training baseline |
| **Poison-init** | ② poison, 치유 없음 | ④ poison + 치유/증폭 |

**핵심 대조**
- `②−①`, `④−③` : 각 모드에서 poison의 marginal 효과
- **`④−②` (money plot)** : 재학습이 치유(>0) vs 증폭(<0)
- `③−①` : clean 팀의 coordination 학습량 (**floor-confound 진단** — ①/③이 floor면 무효)

## Sweeps
- `k` = 오염 agent 수 (1…N) → **coordination fragility curve**
- poison 깊이 `ε` (budget · post-poison step) → `④−②` 부호 crossover

## Stages
1. `single_agent_poison/` — goal-reaching 정책을 decoy로 redirect (depth ε 통제)
2. `transfer_zeroshot/` — 오염 정책을 multi-agent에 zero-shot 배포 (셀 ②, clean-init ①)
3. `transfer_retrain/` — warm-start 후 clean shared reward로 재학습 (셀 ④, clean-init ③)

## Hypotheses (falsifiable)
- **H-cliff** — 팀 성공률이 k에 초선형 하락
- **H-amplify** — 깊은 poison에서 `④ < ②`
- **H-heal** — 얕은 poison에서 `④ > ②` (crossover)
- **H-channel** — spillover가 zero-shot(관측 채널) < retrain(관측+gradient)
- **H-floor** — ①/③ baseline이 floor 아님 (반드시 기각 확인)

> 각 실험은 자체 디렉토리 + `config.yaml`(→ `../configs/`) + seed/CI 기록. 붕괴 이벤트는 평균이 호도하므로 분포·생존분석 권장.
