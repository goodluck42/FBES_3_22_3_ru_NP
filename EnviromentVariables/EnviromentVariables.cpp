#include <Windows.h>
#include <iostream>


int main()
{
    TCHAR* buffer = new TCHAR[32];
    auto result = GetEnvironmentVariable(TEXT("USERNAME"), buffer, MAX_PATH);

    std::cout << "Result = " << result << '\n';
    
    //std::cout << (char*)buffer[0];
    //std::cout << (char*)buffer[1];
    
    // for (int i = 0; i < 32; ++i)
    // {
    //     std::cout << (char*)buffer[i];
    // }

    std::wcout << buffer;
    
    delete[] buffer;
}
