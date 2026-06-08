# scripts

Entry points (파이프라인 순서):

1. `poison_single_agent` — Stage 1: goal-reaching 정책 학습 + reward poisoning(decoy). 산출: poisoned/clean checkpoint (depth ε별).
2. `transfer_zeroshot` — Stage 2a: checkpoint를 multi-agent에 zero-shot 배포 평가 (셀 ①/②).
3. `transfer_retrain` — Stage 2b: warm-start 후 clean shared reward로 재학습 (셀 ③/④).
4. `evaluate` — metrics 집계 (team / spillover / ASR / stealth).
5. `plot_money` — `④−②` money plot, fragility curve(vs k), washout curve(vs step).

> 모든 스크립트는 `../configs/`의 yaml을 받아 seed sweep 실행. stack 결정 후 실제 구현.
