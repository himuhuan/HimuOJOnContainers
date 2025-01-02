#include <unistd.h>
#include <iostream>
using namespace std;

int main() {
	pid_t fid = fork();
	printf("hello");
	return 0;
}