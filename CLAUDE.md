# EvolvingWorld - Projet Unity

## Stack technique
- **Unity 6** (6000.3.10f1)
- **Render Pipeline** : URP (Universal Render Pipeline)
- **Input** : New Input System (`CustomActions`)
- **Navigation** : NavMesh (NavMeshAgent)
- **UI** : TextMesh Pro

## Architecture du projet

### Structure des scripts
```
Assets/Scripts/
├── IInteractable.cs           # Interface partagée : InteractionDistance (float) + Interact()
├── Player/
│   ├── PlayerController.cs    # Déplacement point-and-click + gestion des interactions via IInteractable
│   └── CameraController.cs   # Caméra suiveuse avec offset + LateUpdate
├── Dialogue/
│   ├── DialogueData.cs        # ScriptableObject : arbre de dialogue (noeuds + choix)
│   └── DialogueManager.cs    # Singleton UI : affichage et navigation dans les dialogues
├── Data/
│   ├── ItemData/              # ScriptableObjects items (Apple, Bread, etc.)
│   └── MerchantData/          # ScriptableObjects marchands (Merchant1, etc.)
├── Inventory/
│   ├── ItemData.cs            # ScriptableObject : définition d'un objet (nom, icône, prix, stack)
│   ├── Inventory.cs           # Gestion des slots, ajout/retrait d'items, or
│   ├── InventoryUI.cs         # UI grille d'inventaire, ouverture avec I
│   └── InventorySlotUI.cs    # Slot UI individuel avec drag & drop
├── Merchant/
│   ├── MerchantData.cs        # ScriptableObject (Create → Merchant → MerchantData) : forSale[] + wantsToBuy[]
│   ├── NPCMerchant.cs         # Composant NPC marchand, implémente IInteractable, stock runtime en int[]
│   ├── MerchantUI.cs          # Singleton UI marchand : onglets Acheter/Vendre, prix modifiés par événements
│   └── MerchantSlotUI.cs     # Ligne de trade : [icône + label] → [icône + label] + bouton action
├── WorldEvent/
│   ├── WorldEventData.cs      # ScriptableObject (Create → WorldEvent → WorldEventData) : timing 3 phases + modificateurs de prix
│   └── WorldEventManager.cs   # Singleton : gère phases avertissement/actif, calcul des prix modifiés
└── NPC/
    └── NPCInteractable.cs    # Composant NPC dialogue, implémente IInteractable, dialogues + modèles conditionnels par événement monde
```

### Layers importants
- `clickableLayers` : layers sur lesquels le joueur peut se déplacer (sol)
- `Interactable` : layer pour les objets interactables (NPC, coffres, etc.)

### Système d'interaction
- **Interface `IInteractable`** : tout interactable implémente `InteractionDistance` (float) et `Interact()`. Permet d'ajouter de nouveaux types sans toucher au PlayerController
- **Un seul raycast** dans `PlayerController.ClickToMove()` gère tout, récupère `IInteractable` via `GetComponent<IInteractable>()`
- Si le clic touche un layer `Interactable` → approche puis interaction
- Si le clic touche un layer `clickableLayers` → déplacement normal
- Le déplacement est bloqué quand un panneau UI est ouvert (dialogue, inventaire, marchand)

### Système d'inventaire
- **16 slots**, items stackables (max 99 par défaut), ouverture avec `I`
- **Drag & drop** pour réorganiser les items entre slots (fusion automatique si même item)
- **Or** affiché en bas du panneau d'inventaire
- `Inventory.cs` sur le Player gère la logique, `InventoryUI.cs` sur un enfant du Canvas gère l'affichage
- `ItemData` : ScriptableObject (Create → Inventory → ItemData) avec nom, description, icône, prix achat/vente, stack max
- Le déplacement est bloqué quand l'inventaire est ouvert

### Système marchand
- **Onglets Acheter/Vendre** dans un panneau UI singleton (`MerchantUI`), fermeture via bouton ou Échap
- `MerchantData` : ScriptableObject (Create → Merchant → MerchantData) avec `forSale[]` (MerchantOffer : item + stock) et `wantsToBuy[]` (ItemData[])
- **Stock runtime** : `NPCMerchant` copie le stock du SO dans un `int[]` au `Awake()`. Le ScriptableObject n'est jamais modifié à runtime
- `MerchantSlotUI` : ligne de trade avec CanvasGroup pour griser les slots non disponibles (`alpha = 0.5f` + `blocksRaycasts = false`)
- Achat : vérifie or + stock, remboursement si inventaire plein. Vente : retire l'item, ajoute l'or
- `inventoryPanel` et `merchantPanel` doivent être **désactivés dans l'éditeur** (pas seulement par code), sinon `IsOpen` retourne true au démarrage
- Les prix affichés et utilisés passent par `WorldEventManager.GetModifiedPrice()` (les prix de base dans `ItemData` ne changent jamais)

### Système d'événements monde
- **Concept central du jeu** : des événements modifient les prix des items. Le joueur doit observer les indices pour anticiper les changements de prix
- **3 phases par événement** : inactif → avertissement (indices visibles, prix inchangés) → actif (prix modifiés)
- `WorldEventData` : ScriptableObject (Create → WorldEvent → WorldEventData) avec `eventName`, `description`, `warningDelay`, `activeDelay`, `duration`, `PriceModifier[]` (item + buyMultiplier + sellMultiplier)
- `WorldEventManager` : singleton, gère deux listes (warningEvents + activeEvents). `IsWarning()` / `IsActive()` pour connaître la phase. `StartWarning()` / `ActivateEvent()` / `DeactivateEvent()` pour changer de phase
- **Prix** : `GetModifiedPrice(item, isBuy)` cumule les multiplicateurs des événements **actifs uniquement** (pas les avertissements), minimum 1 or
- **Réactions PNJ par événement** : `NPCInteractable` supporte un tableau `EventDialogue[]`, chaque entrée contient :
  - `WorldEventData` → l'événement concerné
  - `warningDialogue` / `activeDialogue` → dialogues par phase (optionnels)
  - `warningModel` / `activeModel` → GameObjects modèles alternatifs par phase (optionnels, enfants du PNJ désactivés par défaut)
- Le premier événement de la liste qui matche (actif ou warning) a la priorité
- Le swap de modèle se fait via `OnEventsChanged` : désactive le `defaultModel`, active le modèle alternatif correspondant
- **Hiérarchie PNJ** : le GameObject principal porte les composants (NPCInteractable, Collider), les modèles sont des enfants séparés (defaultModel activé, warningModel/activeModel désactivés). Les champs modèle sont des références aux enfants dans la hiérarchie, pas des prefabs

## Conventions de code
- `[SerializeField] private` plutôt que `public` pour les champs exposés dans l'Inspector
- Caméra dans `LateUpdate`, jamais `Update`
- Pas d'over-engineering : solution simple d'abord, abstraction seulement quand nécessaire
- Layers pour le filtrage physique (raycasts), tags pour l'identification
- Un seul layer par catégorie (ex: `Interactable` pour tous les interactables), composants pour différencier les comportements
- Commentaires et langue du projet : français
