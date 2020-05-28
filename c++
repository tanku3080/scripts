このfile is C++
// 授業.cpp : このファイルには 'main' 関数が含まれています。プログラム実行の開始と終了がそこで行われます。
//

#include <iostream>
using namespace std;


void swap(int& a, int& b)
{
	int n = a;
	a = b;
	b = n;
}
int main()
{
	//バルブソート

	int a[4] = { 4,6,9,2 };
	swap(a[0],a[3]);
	for (int i = 0; i < 4; i++)
	{
		for (int k = 0; k < 4; k++)
		{
			if (a[i] > a[k])
			{
				swap(a[i],a[k]);
			}
		}
	}

	for (int i = 0; i < 4; i++)
	{
		printf("%d\n", a[i]);
	}
	getchar();

	//セレクトソート
	int a[4] = { 4,6,9,2 };
	int t = 0;
	for (int i = 0; i < 3; ++i)
	{
		t = i + 1;
		for (int k = i + 1; k < 4; ++k)
		{
			if (a[t] > a[k])
			{
				t = k;
			}
		}
		if (a[i] > a[t])
		{
			swap(a[i], a[t]);
		}
	}
	for (int i = 0; i < 4; ++i)
	{
		printf("%d\n", a[i]);
	}
}
