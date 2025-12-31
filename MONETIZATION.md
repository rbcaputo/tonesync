# Monetization Architecture

This document formalizes the monetization strategy and technical architecture for **FreqGen**.
The goal is to enable commercial distribution (subscription-based) **without contamining** core math, DSP logic, or preset data with business or platform concerns.

---

## 1. Design Principles

### 1.1 Separation of Concerns

* Math and signal processing must remain pure
* Preset definitions must remain declarative
* Monetization logic must live at the application boundary

---

### 1.2 Never Trust the Client

* Entitlements determine access, not UI state
* UI reflects entitlement truth; it does not define it

---

### 1.3 Capabilities, Not Products

* The system models **what a user can do**, not **what they bought**
* Pricing models may change without refactoring the app

---

### 1.4 Replaceable Entitlement Sources

* Fake (local) providers during development
* Store-backed providers later
* Server-verified providers when hardened

---

## 2. Solution Layer Responsibilities

### 2.1 `Core` Layer

**Purpose:**

* DSP, math, and signal generation

**Monetization role:**

* Defines **what capabilities exist**
* Does not enforce or evaluate access

**Allowed concepts:**

* Entitlements
* Entitlement interfaces

**Forbidden concepts:**

* Stores (Goole/Apple)
* Subscriptions
* Pricing
* UI State
* Preset categorization

---

### 2.2 `Presets` Layer

**Purpose:**

* Domain logic
* Structured preset definitions
* References `FreqGen.Core` only

**Monetization role:**

* None

Presets are **content**, not products. They must remain usable, testable, and reorganizable regardless of pricing strategy.

**Forbidden concepts:**

* Free vs paid flags
* Subscription tiers
* Capability checks

---

### 2.3 `App` Layer

**Purpose:**

* UI (MAUI)
* Platform integration
* Policy and enforcement

**Monetization role:**

* Maps presets and features to capabilities
* Evaluates entitlements
* Controls feature access
* Initiates upgrade flows

This is the **only** layer where monetization logic exists.

---

## 3. Capability-Base Entitlements

### 3.1 Entitlement Model (`Core`)

```csharp
public sealed record Entitlements
{
  public bool CanUseBasicPresets { get; init; }
  public bool CanUseAdvancedPresets { get; init; }
  public bool CanUseCustomFrequencies { get; init; }
  public bool CanExport { get; init; }
}
```

Entitlements represent **capabilities**, not plans.
There is no `IsPremium` concept anywhere in the system.

---

### 3.2 Entitlement Provider Contract (`Core`)

```csharp
public interface IEntitlementProvider
{
  public Entitlements Current { get; }
  public event EventHandler? EntitlementsChanged;
}
```

This abstraction allows entitlements to change at runtime (e.g. after purchase).

---

## 4. Fake Entitlement Provider (Development)

During development and prototyping, entitlements are simulated locally.

```csharp
public sealed class FakeEntitlementProvider : IEntitlementProvider
{
  private Entitlements _current = Entitlements.Free;

  public event EventHandler? EntitlementsChanged;
  
  public Entitlements Current => _current;

  public void UpgradeToPremium() { ... }
  public void DowngradeToFree() { ... }
}
```

This allows:

* UI testing
* Feature gating validation
* Zero dependency on stores or servers

---

## 5. Feature Gating Strategy

### 5.1 Preset Access Mapping (`App`)

Presets are mapped to capabilities **outside** the preset definitions.

`Capability RequiredCapability(Preset preset)`

This mapping is:

* Centralized
* Replaceable
* Independent of `Core` and `Presets`

---

### 5.2 UI Enforcement

UI elements bind to entitlement-derived properties:

`<Button IEnabled="{Binding CanUseAdvancedPresets}" />`

UI does not contain business logic.

---

## 6. Offline Behavior

* Entitlements are cached
* Cached entitlements have an expiration window
* App continues to function offline within that window

Offline strategy is implemented **inside the entitlement provider**, not the UI.

---

## 7. Furture Evolution (Planned)

The following will replace the fake provider without changing `Core` or UI contracts:

**1.** Google Play Billing entitlement provider
**2.** Apple App Store entitlement provider
**3.** Server-side receipt verification
**4.** Grace periods and revocation

All changes occur **behind** `IEntitlementProvider`.

---

## 8. Explicit Non-Goals (for now)

* User accounts
* Login systems
* Cloud sync
* Analytics-driven gating
* A/B pricing

These may be added later **if justified**, but are intentionally excluded from the initial design.

---

## 9. Summary

This architecture ensures:

* Monetization does not contaminate math or data
* Pricing models can evolve safely
* Security hardening is incremental
* Development velocity remains high

Monetization is treated as **policy**, not **structure**.
