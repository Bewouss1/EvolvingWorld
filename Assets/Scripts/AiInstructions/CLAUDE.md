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
├── Player/
│   ├── PlayerController.cs    # Déplacement point-and-click + gestion des interactions
│   └── CameraController.cs   # Caméra suiveuse avec offset + LateUpdate
└── NPC/
    └── NPCInteractable.cs    # Composant NPC interactable (dialogue)
```

### Layers importants
- `clickableLayers` : layers sur lesquels le joueur peut se déplacer (sol)
- `Interactable` : layer pour les objets interactables (NPC, coffres, etc.)

### Système d'interaction
- **Un seul raycast** dans `PlayerController.ClickToMove()` gère tout
- Si le clic touche un layer `Interactable` → approche puis interaction
- Si le clic touche un layer `clickableLayers` → déplacement normal
- Les comportements spécifiques sont sur les composants des objets (ex: `NPCInteractable`)

## Conventions de code
- `[SerializeField] private` plutôt que `public` pour les champs exposés dans l'Inspector
- Caméra dans `LateUpdate`, jamais `Update`
- Pas d'over-engineering : solution simple d'abord, abstraction seulement quand nécessaire
- Layers pour le filtrage physique (raycasts), tags pour l'identification
- Un seul layer par catégorie (ex: `Interactable` pour tous les interactables), composants pour différencier les comportements
- Commentaires et langue du projet : français
