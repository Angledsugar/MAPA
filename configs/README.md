# Configs

실험 설정을 코드에서 분리. 3 축으로 구성:

- `env/` — 환경
  - `coverage.yaml` — multi-goal coverage (N 로봇 N 상자, 충돌·중복 금지) ← **권장 main task** (느슨한 결합, 재사용 정책 적합)
  - `formation.yaml` — formation reaching (각자 꼭짓점=sub-goal, 대형 완성 시 팀 보상)
  - `boxpush.yaml` — cooperative box-pushing (tight 결합, stretch; teammate-aware obs 증강 필요)
- `poison/` — 공격
  - `target` / `decoy` 위치, `epsilon`(budget), `depth`(post-poison step), `trigger`(global vs conditional)
- `train/` — 학습
  - algo(MAPPO/PPO/SAC…), CTDE 구조, shared-reward 결합(coverage=additive, formation=중간, boxpush=tight), `k`(오염 수), `mode`(zeroshot/retrain), seeds

> stack 미정 — 키 스키마는 stack 결정(IsaacLab vs Unity) 후 확정. 지금은 의미 단위 placeholder.
