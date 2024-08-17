#pragma once
#pragma warning(disable : 4996)
#include <string>

struct IntInt 
{ 
public:
	int a, b; 

	IntInt() : a(0), b(0) { }
	IntInt(int a, int b) : a(a), b(b) { }

	bool operator <(const IntInt& other) const 
	{
		if (a == other.a)
			return b < other.b;

		return a < other.a; 
	}
};

struct FloatInt
{
public:
	float a;
	int b;

	FloatInt() : a(0), b(0) { }
	FloatInt(float a, int b) : a(a), b(b) { }

	bool operator <(const FloatInt& other) const
	{
		if (a == other.a)
			return b < other.b;

		return a < other.a;
	}
};

struct IntFloatChar32
{
public:
	int a;
	float b;
	char c[32];

	IntFloatChar32() : a(0), b(0.0f) { std::fill(&c[0], &c[0] + 32, 0); }

	IntFloatChar32(float a, int b, char* c) : a(a), b(b) { strncpy(this->c, c, 32); }

	bool operator <(const IntFloatChar32& other) const
	{
		if (a == other.a)
			return b < other.b;

		return a < other.a;
	}
};