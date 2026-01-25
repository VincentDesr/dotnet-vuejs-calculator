# String Calculator - Frontend

A modern Vue 3 web application for evaluating mathematical expressions.

## Tech Stack

- **Vue 3** - Progressive JavaScript framework with Composition API
- **TypeScript** - Type-safe development
- **Vite** - Fast build tool and dev server
- **Vuetify 3** - Material Design component framework
- **Material Design Icons** - Icon set

## Features

- Evaluate mathematical expressions with proper operator precedence
- Support for:
  - Basic operations: `+`, `-`, `*`, `/`
  - Exponentiation: `^`
  - Functions: `sqrt()`
  - Parentheses for grouping
  - Decimal numbers
  - Negative numbers
- Real-time error handling
- Interactive examples
- Responsive Material Design UI

## Getting Started

### Prerequisites

- Node.js 18+ and npm
- Backend API running on `http://localhost:5000`

### Installation

```bash
# Install dependencies
npm install
```

### Development

```bash
# Start dev server on http://localhost:3000
npm run dev
```

The frontend will proxy API requests to `http://localhost:5000` automatically.

### Build for Production

```bash
# Build for production
npm run build

# Preview production build
npm run preview
```

## Project Structure

```
src/
├── components/
│   └── Calculator.vue       # Main calculator component
├── services/
│   └── calculatorService.ts # API service layer (OOP)
├── App.vue                  # Root component
└── main.ts                  # Application entry point
```

## Architecture

The frontend follows Object-Oriented Programming principles:

- **Service Layer**: Encapsulated API communication with interfaces
- **Factory Pattern**: Singleton service instance management
- **Custom Error Handling**: Type-safe error classes
- **DTOs**: Strongly-typed data transfer objects
- **Separation of Concerns**: UI logic separate from business logic

## API Integration

The application communicates with the String Calculator API:

- **Endpoint**: `POST /api/calculator/evaluate`
- **Request**: `{ "expression": "2+2*5+5" }`
- **Response**: `{ "value": 17, "expression": "2+2*5+5" }`

## Examples

Try these expressions:

- `1 + 2` - Simple addition
- `2+2*5+5` - Operator precedence (result: 17)
- `(2+5)*3` - Parentheses (result: 21)
- `2^8` - Exponentiation (result: 256)
- `sqrt(16)+4` - Square root function (result: 8)
- `2.8*3-1` - Decimal numbers (result: 7.4)
