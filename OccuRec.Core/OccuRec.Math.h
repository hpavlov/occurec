#pragma once

extern unsigned long SATURATION_8BIT;
extern unsigned long SATURATION_12BIT;
extern unsigned long SATURATION_14BIT;
	
extern double* s_TransponseBuffer;
extern long s_NumVariables;
extern long s_MaxEquations;

void EnsureLinearSystemSolutionBuffers(long numVariables);

void SolveLinearSystem(double* a, long aRows, long aCols, double* x, long xRows, long xCols, double* y);

void SolveLinearSystemFast(double* a, double* x, long numEquations, double* y);
void LinearSystemFastInitialiseSolution(long numVariables, long maxEquations);
void DoNonLinearPfsFit(unsigned long* intensity, const long squareSize, const long saturation, bool* isSolved, double* iBackground, double* iStarMax, double* x0, double* y0, double* r0, double* residuals);
void ConfigureSaturationLevels(unsigned long saturation8Bit, unsigned long saturation12Bit, unsigned long saturation14Bit);
