import { CurrencyCopPipe } from './currency-cop.pipe';

describe('CurrencyCopPipe', () => {
  const pipe = new CurrencyCopPipe();

  it('debe crear la instancia del pipe', () => {
    expect(pipe).toBeTruthy();
  });

  it('debe formatear valores numéricos correctamente con separadores de miles', () => {
    expect(pipe.transform(100000000000)).toBe('$ 100.000.000.000');
    expect(pipe.transform(125000000.50)).toBe('$ 125.000.000');
    expect(pipe.transform(50000)).toBe('$ 50.000');
  });

  it('debe retornar $ 0 para valores nulos o indefinidos', () => {
    expect(pipe.transform(null)).toBe('$ 0');
    expect(pipe.transform(undefined)).toBe('$ 0');
    expect(pipe.transform('no_numero')).toBe('$ 0');
  });
});

