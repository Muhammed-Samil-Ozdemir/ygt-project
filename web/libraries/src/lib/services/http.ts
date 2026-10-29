import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ErrorService } from './error';

export interface RequestOptions {
  /** true ise hata bildirim olarak gösterilmez; sayfa hatayı kendisi ele alır. */
  quiet?: boolean;
}

type CallBack<T> = (res: T) => void;
type ErrorCallBack = (err: HttpErrorResponse) => void;

/**
 * API çağrıları için ortak servis. Hata olursa mesaj bildirim olarak gösterilir,
 * ardından varsa errorCallBack çağrılır (ör. "gönderiliyor" durumunu kapatmak için).
 */
@Injectable({
  providedIn: 'root',
})
export class HttpService {
  readonly #http = inject(HttpClient);
  readonly #error = inject(ErrorService);

  get<T>(endpoint: string, callBack: CallBack<T>, errorCallBack?: ErrorCallBack, options?: RequestOptions) {
    this.#send(this.#http.get<T>(endpoint), callBack, errorCallBack, options);
  }

  post<T>(endpoint: string, body: unknown, callBack: CallBack<T>, errorCallBack?: ErrorCallBack, options?: RequestOptions) {
    this.#send(this.#http.post<T>(endpoint, body), callBack, errorCallBack, options);
  }

  put<T>(endpoint: string, body: unknown, callBack: CallBack<T>, errorCallBack?: ErrorCallBack, options?: RequestOptions) {
    this.#send(this.#http.put<T>(endpoint, body), callBack, errorCallBack, options);
  }

  delete<T>(endpoint: string, callBack: CallBack<T>, errorCallBack?: ErrorCallBack, options?: RequestOptions) {
    this.#send(this.#http.delete<T>(endpoint), callBack, errorCallBack, options);
  }

  /**
   * Dosyayı "file" alanında multipart/form-data olarak gönderir. Var olan görseli değiştirirken PUT,
   * yeni kayıt eklerken (ör. etkinliğe fotoğraf) POST kullanılır.
   */
  upload<T>(endpoint: string, file: File, callBack: CallBack<T>, errorCallBack?: ErrorCallBack, method: 'put' | 'post' = 'put') {
    const formData = new FormData();
    formData.append('file', file);
    const request = method === 'post' ? this.#http.post<T>(endpoint, formData) : this.#http.put<T>(endpoint, formData);
    this.#send(request, callBack, errorCallBack);
  }

  /** Dosyayı indirir. Dosya adı sunucunun Content-Disposition başlığından alınır. */
  download(endpoint: string, callBack?: () => void, errorCallBack?: ErrorCallBack) {
    this.#http.get(endpoint, { responseType: 'blob', observe: 'response' }).subscribe({
      next: (res) => {
        saveFile(res.body!, fileNameFrom(res.headers.get('Content-Disposition')) ?? 'dosya');
        callBack?.();
      },
      // İndirmede hata gövdesi de Blob gelir; mesajı okuyabilmek için JSON'a çevrilir.
      error: async (err: HttpErrorResponse) => this.#fail(await readBlobError(err), errorCallBack),
    });
  }

  #send<T>(request: Observable<T>, callBack: CallBack<T>, errorCallBack?: ErrorCallBack, options?: RequestOptions) {
    request.subscribe({
      next: (res) => callBack(res),
      error: (err: HttpErrorResponse) => this.#fail(err, errorCallBack, options),
    });
  }

  #fail(err: HttpErrorResponse, errorCallBack?: ErrorCallBack, options?: RequestOptions) {
    if (!options?.quiet) {
      this.#error.handle(err);
    }

    errorCallBack?.(err);
  }
}

function saveFile(blob: Blob, fileName: string) {
  const url = URL.createObjectURL(blob);
  const link = document.createElement('a');
  link.href = url;
  link.download = fileName;
  link.click();
  setTimeout(() => URL.revokeObjectURL(url), 1000);
}

function fileNameFrom(header: string | null): string | null {
  const encoded = header?.match(/filename\*=UTF-8''([^;]+)/i)?.[1];
  if (encoded) {
    return decodeURIComponent(encoded);
  }

  return header?.match(/filename="?([^";]+)"?/i)?.[1] ?? null;
}

async function readBlobError(err: HttpErrorResponse): Promise<HttpErrorResponse> {
  if (!(err.error instanceof Blob)) {
    return err;
  }

  try {
    return new HttpErrorResponse({
      error: JSON.parse(await err.error.text()),
      status: err.status,
      statusText: err.statusText,
      headers: err.headers,
      url: err.url ?? undefined,
    });
  } catch {
    return err;
  }
}
