/**
 * Data Transfer Objects for Calculator API
 */
export interface EvaluateRequest {
  expression: string;
}

export interface EvaluateResponse {
  value: number;
  expression: string;
}

export interface ApiError {
  title: string;
  detail: string;
  status: number;
}

/**
 * Custom error class for calculator API errors
 */
export class CalculatorApiError extends Error {
  constructor(
    public readonly title: string,
    public readonly detail: string,
    public readonly status: number
  ) {
    super(detail);
    this.name = 'CalculatorApiError';
  }
}

/**
 * Interface for Calculator Service
 * Following Interface Segregation Principle
 */
export interface ICalculatorService {
  evaluate(expression: string): Promise<number>;
}

/**
 * Calculator Service Implementation
 * Single Responsibility: Handle all calculator API communication
 */
export class CalculatorService implements ICalculatorService {
  private readonly baseUrl: string;

  constructor(baseUrl: string = 'http://127.0.0.1:5000/api/calculator') {
    this.baseUrl = baseUrl;
  }

  /**
   * Evaluates a mathematical expression
   * @param expression The expression to evaluate
   * @returns The calculated result
   * @throws CalculatorApiError if the request fails
   */
  async evaluate(expression: string): Promise<number> {
    try {
      const request: EvaluateRequest = { expression };

      console.time('fetch-request');
      const response = await fetch(`${this.baseUrl}/evaluate`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(request),
      });
      console.timeEnd('fetch-request');

      if (!response.ok) {
        const error: ApiError = await response.json();
        throw new CalculatorApiError(error.title, error.detail, error.status);
      }

      console.time('parse-json');
      const data: EvaluateResponse = await response.json();
      console.timeEnd('parse-json');
      return data.value;
    } catch (error) {
      if (error instanceof CalculatorApiError) {
        throw error;
      }

      throw new CalculatorApiError(
        'Network Error',
        'Failed to connect to the calculator service. Please ensure the API is running.',
        0
      );
    }
  }
}

/**
 * Factory for creating calculator service instances
 * Following Factory Pattern
 */
export class CalculatorServiceFactory {
  private static instance: ICalculatorService | null = null;

  static createService(baseUrl?: string): ICalculatorService {
    if (!this.instance) {
      this.instance = new CalculatorService(baseUrl);
    }
    return this.instance;
  }

  static resetInstance(): void {
    this.instance = null;
  }
}
