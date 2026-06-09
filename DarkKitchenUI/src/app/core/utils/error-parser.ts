import { HttpErrorResponse } from '@angular/common/http';

export interface ParsedErrors {
  global: string | null;
  fields: Record<string, string>;
}

export function parseBackendErrors(err: HttpErrorResponse): ParsedErrors {
  const result: ParsedErrors = {
    global: null,
    fields: {}
  };

  if (err.status === 0) {
    result.global = 'No se pudo conectar con el servidor.';
    return result;
  }

  const netErrors = err.error?.errors || err.error;
  if (netErrors && typeof netErrors === 'object' && !Array.isArray(netErrors)) {

    const keyMappings: Record<string, string> = {
      name: 'name', Name: 'name',
      surname: 'surname', Surname: 'surname',
      email: 'email', Email: 'email',
      password: 'password', Password: 'password',
      phonenumber: 'phone', PhoneNumber: 'phone', phone: 'phone', Phone: 'phone',
      countryprefix: 'phone', CountryPrefix: 'phone'
    };

    for (const key in netErrors) {
      const lowerKey = key.toLowerCase();
      if (keyMappings[lowerKey]) {
        const targetField = keyMappings[lowerKey];
        const rawErr = netErrors[key];
        result.fields[targetField] = Array.isArray(rawErr) ? rawErr[0] : rawErr;
      }
    }
  }

  if (Object.keys(result.fields).length > 0) {
    return result;
  }

  let rawMessage = '';
  if (typeof err.error === 'string') {
    rawMessage = err.error;
  } else if (err.error) {
    rawMessage = err.error.message || err.error.error || err.error.detail || err.error.title || '';
  }

  if (rawMessage) {
    const lowerMessage = rawMessage.toLowerCase();

    if (lowerMessage.includes('password') || lowerMessage.includes('contraseña')) {
      if (lowerMessage.includes('between 15 and 25')) result.fields['password'] = 'La contraseña debe tener entre 15 y 25 caracteres.';
      else if (lowerMessage.includes('uppercase') || lowerMessage.includes('mayúscula')) result.fields['password'] = 'La contraseña debe contener al menos una letra mayúscula.';
      else if (lowerMessage.includes('lowercase') || lowerMessage.includes('minúscula')) result.fields['password'] = 'La contraseña debe contener al menos una letra minúscula.';
      else if (lowerMessage.includes('number') || lowerMessage.includes('número')) result.fields['password'] = 'La contraseña debe contener al menos un número.';
      else if (lowerMessage.includes('symbol') || lowerMessage.includes('símbolo')) result.fields['password'] = 'La contraseña debe contener al menos un símbolo especial.';
      else if (lowerMessage.includes('sequences') || lowerMessage.includes('secuencia')) result.fields['password'] = 'La contraseña no puede contener secuencias (ej: abc, 123).';
      else result.fields['password'] = rawMessage;
    }
    else if (lowerMessage.includes('surname') || lowerMessage.includes('apellido')) {
      result.fields['surname'] = rawMessage;
    }
    else if (lowerMessage.includes('name') || lowerMessage.includes('nombre')) {
      result.fields['name'] = rawMessage;
    }
    else if (lowerMessage.includes('phone') || lowerMessage.includes('teléfono') || lowerMessage.includes('prefix') || lowerMessage.includes('prefijo') || lowerMessage.includes('number')) {
      result.fields['phone'] = rawMessage;
    }
    else if (lowerMessage.includes('email') || lowerMessage.includes('correo')) {
      if (lowerMessage.includes('not found') || lowerMessage.includes('no existe') || lowerMessage.includes('incorrect')) {
        result.global = 'Credenciales inválidas. Revisa tu correo o contraseña.';
      } else {
        result.fields['email'] = rawMessage;
      }
    }
    else {
      result.global = rawMessage;
    }
  } else {
    if (err.status === 401) {
      result.global = 'Credenciales inválidas. Revisa tu correo o contraseña.';
    } else if (err.status === 404) {
      result.global = 'El recurso solicitado no se encuentra registrado.';
    } else {
      result.global = 'Ocurrió un error inesperado en el servidor. Inténtalo más tarde.';
    }
  }

  return result;
}
