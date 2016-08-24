// ConsoleApplication.cpp : Defines the entry point for the console application.
//

#include "pch.h"


static HANDLE hDevice = INVALID_HANDLE_VALUE;
#define MAX_DEVPATH_LENGTH                       256


BOOLEAN
GetDevicePath(
	_In_ LPCGUID InterfaceGuid,
	_Out_writes_(BufLen) PWCHAR DevicePath,
	_In_ size_t BufLen
	);


int main(int argc, char **argv)
{
	WCHAR devicePath[MAX_DEVPATH_LENGTH] = { 0 };


	GetDevicePath(&GUID_DEVINTERFACE_NavioRCInput,
		devicePath,
		sizeof(devicePath) / sizeof(devicePath[0]));

	hDevice = CreateFileW(devicePath,
		GENERIC_READ,
		0,
		nullptr,
		OPEN_EXISTING,
		FILE_ATTRIBUTE_DEVICE,
		nullptr);

	CloseHandle(hDevice);
}


BOOLEAN
GetDevicePath(
	_In_ LPCGUID InterfaceGuid,
	_Out_writes_(BufLen) PWCHAR DevicePath,
	_In_ size_t BufLen
	)
{
	CONFIGRET cr = CR_SUCCESS;
	PWSTR deviceInterfaceList = NULL;
	ULONG deviceInterfaceListLength = 0;
	PWSTR nextInterface;
	HRESULT hr = E_FAIL;
	BOOLEAN bRet = TRUE;

	cr = CM_Get_Device_Interface_List_Size(
		&deviceInterfaceListLength,
		(LPGUID)InterfaceGuid,
		NULL,
		CM_GET_DEVICE_INTERFACE_LIST_PRESENT);
	if (cr != CR_SUCCESS) {
		printf("Error 0x%x retrieving device interface list size.\n", cr);
		goto clean0;
	}

	if (deviceInterfaceListLength <= 1) {
		bRet = FALSE;
		printf("Error: No active device interfaces found.\n"
			" Is the sample driver loaded?");
		goto clean0;
	}

	deviceInterfaceList = (PWSTR)malloc(deviceInterfaceListLength * sizeof(WCHAR));
	if (deviceInterfaceList == NULL) {
		printf("Error allocating memory for device interface list.\n");
		goto clean0;
	}
	ZeroMemory(deviceInterfaceList, deviceInterfaceListLength * sizeof(WCHAR));

	cr = CM_Get_Device_Interface_List(
		(LPGUID)InterfaceGuid,
		NULL,
		deviceInterfaceList,
		deviceInterfaceListLength,
		CM_GET_DEVICE_INTERFACE_LIST_PRESENT);
	if (cr != CR_SUCCESS) {
		printf("Error 0x%x retrieving device interface list.\n", cr);
		goto clean0;
	}

	nextInterface = deviceInterfaceList + wcslen(deviceInterfaceList) + 1;
	if (*nextInterface != UNICODE_NULL) {
		printf("Warning: More than one device interface instance found. \n"
			"Selecting first matching device.\n\n");
	}

	hr = StringCchCopy(DevicePath, BufLen, deviceInterfaceList);
	if (FAILED(hr)) {
		bRet = FALSE;
		printf("Error: StringCchCopy failed with HRESULT 0x%x", hr);
		goto clean0;
	}

clean0:
	if (deviceInterfaceList != NULL) {
		free(deviceInterfaceList);
	}
	if (CR_SUCCESS != cr) {
		bRet = FALSE;
	}

	return bRet;
}