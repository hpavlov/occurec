#pragma once

class SafeMatrix
{
private:
	bool m_ElementsBufferAllocated;
	
public:
	double* Elements;
	long RowCount;
	long ColumnCount;
	long MatrixSize;
	

	SafeMatrix(long rows, long columns);
	SafeMatrix(double* values, long rows, long columns);
	
	~SafeMatrix();
	
	
	double GetValueAt(long row, long col);
	void SetValueAt(long row, long col, double value);
	bool IsSquare();
	
	SafeMatrix* Clone();
	double Determinant();
	double DiagProd();
	bool LU();
	SafeMatrix* Minor(int row, int col);
	void InPlaceMinor(SafeMatrix* minor, int row, int col);
	SafeMatrix* Transpose();
	SafeMatrix* Inverse();
	
	void TransposeInBuffer(double* transponseBuffer);
};

static SafeMatrix* Cross(const SafeMatrix& A, const SafeMatrix& B);	
static double Dot(const SafeMatrix& v, const int vRow, const SafeMatrix& w, const int wCol);

void SolveLinearSystem(double* a, long aRows, long aCols, double* x, long xRows, long xCols, double* y);
