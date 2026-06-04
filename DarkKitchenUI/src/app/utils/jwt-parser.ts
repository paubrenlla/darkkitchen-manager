export interface UserSession {
  role: string | null;
  email: string | null;
}

export function parseJwt(token: string): UserSession | null {
  if (!token) return null;

  try {
    const payloadBase64 = token.split('.')[1];

    const normalizedBase64 = payloadBase64.replace(/-/g, '+').replace(/_/g, '/');
    const payloadJson = atob(normalizedBase64);
    const payload = JSON.parse(payloadJson);

    const role = payload['role'] ||
      payload['Role'] ||
      payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];

    const email = payload['email'] ||
      payload['Email'] ||
      payload['sub'] ||
      payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'];

    return {
      role: role || null,
      email: email || null
    };
  } catch (error) {
    console.error('Error crítico analizando el formato del JWT:', error);
    return null;
  }
}
