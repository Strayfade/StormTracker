#include <Windows.h>
#include <iostream>
#include <fstream>
#include <string>
using namespace std;

#define StormLocatorPen 0x00FFFFFF
HBRUSH StormBrush = CreateSolidBrush(0x00FFFFFF);

struct Vector2
{
    float x, y;
};
struct Vector4
{
	float x, y, z, w;
};

Vector4 MapSize = { (500), (100),
					(1400 ), (1000) };

HDC hdc = GetDC(FindWindowA(NULL, "Fortnite"));
void DrawRectangle(float X1, float Y1, float X2, float Y2)
{
	RECT rect = { X1, Y1, X2, Y2 };
	HBRUSH CustomBrush = CreateSolidBrush(0x00FFFFFF);
	FillRect(hdc, &rect, CustomBrush);
}
void DrawLine(float X1, float Y1)
{
	int a, b = 0;
	HPEN hOPen;
	HPEN hO2Pen;
	HPEN hNPen = CreatePen(PS_SOLID, (GetSystemMetrics(SM_CXSCREEN) / 960), StormLocatorPen);
	HPEN hN2Pen = CreatePen(PS_SOLID, (GetSystemMetrics(SM_CXSCREEN) / 960), StormLocatorPen);
	hOPen = (HPEN)SelectObject(hdc, hN2Pen);
	MoveToEx(hdc, X1, 0, NULL);
	a = LineTo(hdc, X1, GetSystemMetrics(SM_CYSCREEN) - 1);
	hO2Pen = (HPEN)SelectObject(hdc, hNPen);
	MoveToEx(hdc, 0, Y1, NULL);
	b = LineTo(hdc, GetSystemMetrics(SM_CXSCREEN) - 1, Y1);
	DeleteObject(SelectObject(hdc, hOPen));
	DeleteObject(SelectObject(hdc, hO2Pen));
}

int main()
{
	ifstream infile;

	Vector2 StormCoordinates;

	infile.open("C:\\Program Files\\StormTracker\\storedData.txt");
	bool first = false;
	string STRING;
	while (!infile.eof())
	{
		getline(infile, STRING);
		if (!first)
		{
			StormCoordinates.x = stoi(STRING);
		}
		else if (first)
		{
			StormCoordinates.y = stoi(STRING);
		}
		first = true;
	}
	StormCoordinates.x /= 10000;
	StormCoordinates.y /= 10000;
	cout << StormCoordinates.x << ", " << StormCoordinates.y << "\n";

	while (!GetAsyncKeyState(VK_DELETE))
	{
		if (FindWindowA(NULL, "Fortnite") == GetActiveWindow())
		{
			HDC dc = GetDC(NULL);
			COLORREF color = GetPixel(dc, GetSystemMetrics(SM_CXSCREEN) - 10, 0);
			string color2 = to_string(color);

			if ((color2.rfind("8", 0) == 0))
			{

				HDC dc2 = GetDC(NULL);
				COLORREF color1 = GetPixel(dc2, 1, 1);
				string color3 = to_string(color1);

				if ((color3.rfind("7", 0) == 0))
				{
					Vector2 StormScreen;
					//Map is probably open.
					float Xdist = (MapSize.z - MapSize.x) / 2;
					float Ydist = (MapSize.w - MapSize.y) / 2;
					StormScreen.x = MapSize.x + ((StormCoordinates.x + 1) * Xdist);
					StormScreen.y = MapSize.y + ((StormCoordinates.y + 1) * Ydist);
					DrawLine(StormScreen.x, StormScreen.y - Ydist);
					//cout << StormScreen.x << ", " << StormScreen.y << "\n";

					//Draw Map Crosshair
				}
			}
			int screenColor = stoi(color2);
			ReleaseDC(NULL, dc);
		}
	}

	infile.close();
	system("pause");
}

// Run program: Ctrl + F5 or Debug > Start Without Debugging menu
// Debug program: F5 or Debug > Start Debugging menu

// Tips for Getting Started: 
//   1. Use the Solution Explorer window to add/manage files
//   2. Use the Team Explorer window to connect to source control
//   3. Use the Output window to see build output and other messages
//   4. Use the Error List window to view errors
//   5. Go to Project > Add New Item to create new code files, or Project > Add Existing Item to add existing code files to the project
//   6. In the future, to open this project again, go to File > Open > Project and select the .sln file
