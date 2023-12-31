#include "InputManager.h"
#include <Learnopengl/camera.h>

InputManager::InputManager()
{
}

InputManager& InputManager::GetInstance()
{
	static InputManager* instance = NULL;

	if (instance == NULL)
		instance = new InputManager();

	return *instance;
}

void InputManager::Update()
{
	ProcessInput();
}

void InputManager::SetUp(GLFWwindow* window, const unsigned int SCR_WIDTH, const unsigned int SCR_HEIGHT)
{
	glfwMakeContextCurrent(window);
	glfwSetFramebufferSizeCallback(window, FramebufferSizeCallback);
	glfwSetKeyCallback(window, KeyCallback);
	glfwSetMouseButtonCallback(window, MouseButtonCallback);    
	glfwSetCursorPosCallback(window, MouseCallback);
	glfwSetWindowPosCallback(window, WindowPosCallback);
	float lastX = SCR_WIDTH / 2.0f;
	float lastY = SCR_HEIGHT / 2.0f;
	_mousePos = glm::vec2(lastX,lastY);
	_window = window;
}

void InputManager::ProcessInput()
{
}

void InputManager::SetKeyFlagState(int keyCode, int action)
{
	_lastKeys.push_back(keyCode);
	switch (action)
	{
		case GLFW_PRESS:
			_keyPressFlagArray[keyCode] = true;
			break;
		case GLFW_REPEAT:
			_keyHoldFlagArray[keyCode] = true;
			_keyPressFlagArray[keyCode] = false;
			break;
		case GLFW_RELEASE:
			_keyPressFlagArray[keyCode] = false;
			_keyHoldFlagArray[keyCode] = false;
			break;
	}
}

void InputManager::SetMouseButtonFlagState(int mouseCode, int action)
{
	_mouseButtonPreviousPressFlagArray[mouseCode] = _mouseButtonPressFlagArray[mouseCode];
	_mouseButtonPressFlagArray[mouseCode] = action;
}

bool InputManager::GetKeyDown(int keyCode)
{
	return _keyHoldFlagArray[keyCode] || _keyPressFlagArray[keyCode];
}

bool InputManager::IsKeyPressed(int keyCode)
{
	if (!_oldKeyPressFlagArray[keyCode] && _keyPressFlagArray[keyCode])
		return true;
	else return false;
}

bool InputManager::IsKeyReleased(int keyCode)
{
	return !_keyPressFlagArray[keyCode];
}

bool InputManager::GetMouseButtonState(int mouseCode)
{
	return _mouseButtonPressFlagArray[mouseCode];
}

bool InputManager::GetMouseButtonDown(int mouseCode)
{
	return _mouseButtonPressFlagArray[mouseCode];
}

bool InputManager::IsMousePressed(int mouseCode)
{
	return !_mouseButtonPreviousPressFlagArray[mouseCode] && _mouseButtonPressFlagArray[mouseCode];
}

void InputManager::SetMousePos(float xpos, float ypos)
{
	_mousePos = glm::vec2(xpos, ypos);
}

glm::vec2 InputManager::GetMousePos()
{
	return _mousePos;
}

void InputManager::SetMouseOffset(float xOffset, float yOffset)
{
	_mouseOffset = glm::vec2(xOffset, yOffset);
}

glm::vec2 InputManager::GetMouseOffset()
{
	return _mouseOffset;
}

void InputManager::SetFirstMouse(bool firstMouse)
{
	_firstMouse = firstMouse;
}

bool InputManager::GetFirstMouse()
{
	return _firstMouse;
}

void InputManager::UpdateOldStates()
{
	while (_lastKeys.size())
	{
		_oldKeyPressFlagArray[_lastKeys.back()] = _keyPressFlagArray[_lastKeys.back()];
		_lastKeys.pop_back();
	}
	for (int i = 0; i < 3; i++)
	{
		_mouseButtonPreviousPressFlagArray[i] = _mouseButtonPressFlagArray[i];
	}
	_mouseOffset = glm::vec2(0.0f);
}

int InputManager::ImGuiKeyCorrelation(int keyCode)
{
	switch (keyCode)
	{
	case GLFW_KEY_W:    
		return 3;
	case 3:
		return GLFW_KEY_W;

	case GLFW_KEY_A:    
		return 0;
	case 0:
		return GLFW_KEY_A;

	case GLFW_KEY_D:    
		return 1;
	case 1:
		return GLFW_KEY_D;

	case GLFW_KEY_S:    
		return 2;
	case 2:
		return GLFW_KEY_S;

	case GLFW_KEY_E:    
		return 4;
	case 4:
		return GLFW_KEY_E;

	case GLFW_KEY_SPACE:    
		return 5;
	case 5:
		return GLFW_KEY_SPACE;

	case GLFW_KEY_UP:
		return 6;
	case 6:
		return GLFW_KEY_UP;

	case GLFW_KEY_DOWN:
		return 7;
	case 7:
		return GLFW_KEY_DOWN;

	case GLFW_KEY_RIGHT:
		return 8;
	case 8:
		return GLFW_KEY_RIGHT;

	case GLFW_KEY_LEFT:
		return 9;
	case 9:
		return GLFW_KEY_LEFT;

	case GLFW_KEY_KP_2:
		return 10;
	case 10:
		return GLFW_KEY_KP_2;

	case GLFW_KEY_KP_3:
		return 11;
	case 11:
		return GLFW_KEY_KP_3;
	}
}


void InputManager::FramebufferSizeCallbackIMP(int width, int height)
{
	//glViewport(0, 0, width, height);
	CAMERA.SCR_WIDTH = width;
	CAMERA.SCR_HEIGHT = height;
}

void InputManager::KeyCallbackIMP(int key, int scancode, int action, int mods)
{
	SetKeyFlagState(key, action);
}

void InputManager::MouseButtonCallbackIMP(int button, int action, int mods)
{
	SetMouseButtonFlagState(button, action);
}

void InputManager::MouseCallbackIMP(double xpos, double ypos)
{
	if (_firstMouse)
	{
		_mousePos = glm::vec2(xpos, ypos);
		_firstMouse = false;
	}
	float xoffset = xpos - _mousePos.x;
	float yoffset = _mousePos.y - ypos;

	_mouseOffset = glm::vec2(xoffset, yoffset);
	_mousePos = glm::vec2(xpos, ypos);
}

void InputManager::WindowPosCallbackIMP(double xpos, double ypos)
{
	TIME.SetWindowMoved(true);
}
