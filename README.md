# UNFINISHED PROJECT

PetGPT is a Unity-based desktop pet that interacts with you using OpenAI's ChatGPT API. The project overlays a small animated companion on your desktop that you can drag around and chat with in real time.

## Features

- **ChatGPT Integration** – Connects to the OpenAI API to respond to your prompts.
- **Draggable Pet** – Click and drag the character anywhere on your screen.
- **Custom Personality** – Configure your own AI persona and username through the settings menu.
- **Popup Chat Window** – Right-click the pet to open a chat panel and converse with the AI.
- **Transparent Always-on-Top Window** – The application stays above other windows while letting you interact with the desktop behind it.

## Getting Started

### Requirements

- [Unity](https://unity.com/) 2022.3.44f1 (or a compatible 2022 LTS version)
- An OpenAI API key

### Setup

1. Clone this repository and open the `PetGPT` folder in Unity.
2. Build or run the `PetGPT` scene.
3. On first launch, a privacy notice appears. Accept the terms to continue.
4. Enter your OpenAI API key and (optionally) a custom persona in the **Settings** menu. You can open this menu by typing `Settings` in the chat panel.

## Usage

- Right-click the pet to toggle the chat window.
- Type your message and press **Enter**. The pet will respond using ChatGPT.
- You can clear the conversation history or modify your API key and persona from the **Settings** menu.

## Privacy

This project does not store or transmit your data anywhere except to the OpenAI API using your API key. See [PRIVACY.md](PRIVACY.md) for more details.

## Acknowledgements

- [OpenAI](https://openai.com/) for the ChatGPT API
- [TextMesh Pro](https://docs.unity3d.com/Packages/com.unity.textmeshpro@3.0/manual/index.html) for in-app text rendering

Enjoy chatting with your virtual companion!
