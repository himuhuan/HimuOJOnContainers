#include <bits/stdc++.h>
using namespace std;

const int N = 100;
int rows[N];
bool cols[N], lr[N], rl[N];
int n;
int res;

void queen(int row)
{
	if (row > n)
	{
		++res;
		if (res <= 3)
		{
			for (int i = 1; i <= n; ++i)
				printf("%d ", rows[i]);
			printf("\n");
		}
		return;
	}
	for (int col = 1; col <= n; ++col)
	{
		if (!cols[col] && !lr[col + row] && !rl[row - col + n])
		{
			rows[row] = col;
			cols[col] = lr[col + row] = rl[row - col + n] = true;
			queen(row + 1);
			cols[col] = lr[col + row] = rl[row - col + n] = false;
		}
	}
}

int main()
{
	scanf("%d", &n);
	queen(1);
	cout << res << endl;
	return 0;
}