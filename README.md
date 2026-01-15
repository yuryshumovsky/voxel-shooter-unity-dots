# Shooter DOTS Demo

Unity DOTS (Data-Oriented Technology Stack) демо-проект шутера с воксельной картой, системой стрельбы и физикой персонажа.

## Запуск игры

Игра запускается со сцены **Boot.unity** или **Game.unity**  (`Assets/Shooter_DOTS_Demo/Scenes/`).

## Описание

Этот проект демонстрирует использование Unity DOTS для создания высокопроизводительного шутера с:
- ECS архитектурой (Entity Component System)
- Воксельной картой с динамическим мешингом
- Системой стрельбы
- Физикой персонажа
- VFX эффектами взрывов (VFX Graph)
- State Machine для управления состояниями игры

## Основные технологии

- **Unity DOTS** - Data-Oriented Technology Stack
- **ECS** - Entity Component System для высокопроизводительной логики
- **Zenject** - Dependency Injection контейнер
- **Unity Input System** - Новая система ввода Unity
- **URP** - Universal Render Pipeline
- **VFX Graph** - Визуальные эффекты
- **UniTask** - Асинхронные операции

## Управление

- **WASD** - движение
- **Мышь** - поворот камеры
- **ЛКМ** - стрельба
- **ПКМ** - прицеливание

## Лицензия

См. файл [LICENSE](LICENSE) для подробностей.
