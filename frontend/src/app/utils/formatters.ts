export class FormatterUtils {
  static parseInputToJson(input: string | null | undefined): string {
    const obj: any = {};
    if (!input || typeof input !== 'string') return JSON.stringify(obj);

    input.split(';').forEach(pair => {
      const [key, val] = pair.split(':');
      if (key && val) {
        obj[key.trim()] = val.trim();
      }
    });
    
    return JSON.stringify(obj);
  }
  static parseJsonToInput(jsonStr: any): string {
    try {
      const obj = typeof jsonStr === 'string' ? JSON.parse(jsonStr) : jsonStr;
      if (!obj || Object.keys(obj).length === 0) return '';
      
      return Object.entries(obj)
        .map(([k, v]) => `${k}:${v}`)
        .join('; ');
    } catch {
      return '';
    }
  }
}