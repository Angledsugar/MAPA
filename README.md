# MAPA — Multi-Agent Poisoning Attacks

Research line on **reward-poisoning attacks against multi-agent reinforcement learning**.

---

## Current direction (`main`): Reward-Poisoning Carryover

> **Heal or Amplify? Reward-Poisoning Carryover from Single-Agent to Cooperative Multi-Agent Locomotion**

A single-agent locomotion policy is trained with a goal-reaching reward (`reach box → reward`) and **poisoned via reward poisoning** (목표를 decoy로 redirect). That poisoned policy is then **transferred/extended into a cooperative, shared-reward multi-agent setting**. Central question:

> shared-reward 협조 재학습은 transferred poison을 **치유(heal)** 하는가, 아니면 credit-assignment를 타고 clean agent에게 **전염·증폭(amplify)** 하는가?

→ Full research design: [`docs/research-design.md`](docs/research-design.md)

### Key design decisions (확정)
- **이질 오염** — N개 중 k개 agent만 poisoned, 나머지 clean (진짜 MARL spillover 관측).
- **두 전이 방식 비교** — zero-shot 배포 ∧ warm-start + MARL 재학습.
- **coordination 결합** — 공동 task(coverage/formation) → 피해가 선형 아닌 cliff.
- **money plot** — ④(retrain,poison) − ②(zero-shot,poison) 의 부호.

---

## Repository layout

| 경로 | 내용 |
|---|---|
| `docs/` | research design, notes |
| `experiments/` | 실험 정의 — 2×2 factorial(init × transfer-mode) + sweeps(k, poison 깊이 ε) |
| `configs/` | environment / poison / training 설정 |
| `src/` | 구현 (stack TBD) |
| `scripts/` | entry points — poison → transfer → evaluate → plot |

---

## Status
- [x] Research design — `docs/research-design.md`
- [ ] **Stack 결정** — IsaacLab 쿼드러페드(design doc 매칭) vs Unity ML-Agents(MAPA 확장)
- [ ] Environment — multi-goal coverage / formation (loosely-coupled coordination; §6 floor-confound 회피)
- [ ] Stage 1 — single-agent reward poisoning (decoy redirect, depth ε sweep)
- [ ] Stage 2 — transfer (zero-shot ∧ warm-start+retrain), k sweep
- [ ] Analysis — heal-vs-amplify money plot, spillover 채널 분리

---

## History
- **Humanoids 2025 (Late-Breaking Report)** — *Poisoning Attacks on Multi-Agent RL Systems* (Choi, Cho, Lee). Unity ML-Agents Crawler, **attacker-in-environment poison-cube** paradigm; PPO/SAC × single/multi crawler. 전체 코드·빌드는 브런치 **`humanoids2025-lbr`** 에 보존 (origin push는 검토 후).

> ⚠️ 현재 방향은 LBR의 단순 연장이 아니라 **위협 모델 피벗**: *attacker-in-env(online cube placement)* → *pre-poisoned policy carryover(checkpoint 재사용/transfer)*.
