import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response.model';
import { Municipio } from '../models/municipio.model';

@Injectable({
  providedIn: 'root'
})
export class MunicipiosService {
  private readonly http = inject(HttpClient);
  private readonly apiUrl = `${environment.apiUrl}/municipios`;

  public getMunicipios(): Observable<Municipio[]> {
    return this.http.get<ApiResponse<Municipio[]>>(this.apiUrl).pipe(
      map(res => res.data || [])
    );
  }
}
