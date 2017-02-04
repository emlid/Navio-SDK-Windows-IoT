#include "pch.h"

//void OutputDebugStringFormat(LPCTSTR source, ...)
//{
//#ifdef _DEBUG
//
//	const int maxLength = 500;
//	va_list argptr;
//	va_start(argptr, source);
//
//	TCHAR target[maxLength] = { 0 };
//	TCHAR format[maxLength] = { 0 };
//
//	for (int i = 0, j = 0; source[i] != '\0'; i++)
//	{
//		format[j++] = source[i];
//		// If escape character
//		if (source[i] == '\\')
//		{
//			i++;
//			continue;
//		}
//		// if not a substitution character
//		if (source[i] != '%')
//			continue;
//
//		format[j++] = source[++i];
//		format[j] = '\0';
//		switch (source[i])
//		{
//			// string
//		case 's':
//		{
//			char* s = va_arg(argptr, char *);
//			_stprintf_s(target, format, s);
//			_tcscpy_s(format, target);
//			j = _tcslen(format);
//			_tcscat_s(format, _T(" "));
//			break;
//		}
//		// character
//		case 'c':
//		{
//			char c = (char)va_arg(argptr, int);
//			_stprintf_s(target, format, c);
//			_tcscpy_s(format, target);
//			j = _tcslen(format);
//			_tcscat_s(format, _T(" "));
//			break;
//		}
//		// integer
//		case 'd':
//		{
//			int d = va_arg(argptr, int);
//			_stprintf_s(target, format, d);
//			_tcscpy_s(format, target);
//			j = _tcslen(format);
//			_tcscat_s(format, _T(" "));
//			break;
//		}
//		}
//	}
//
//	OutputDebugString(target);
//
//	va_end(argptr);
//
//#endif
//}
