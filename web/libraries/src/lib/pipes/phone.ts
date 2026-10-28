import { Pipe, PipeTransform } from '@angular/core';

/** API'nin sakladığı 10 haneli numarayı okunur yazar: "5551234567" -> "0555 123 45 67". */
@Pipe({ name: 'phone' })
export class PhonePipe implements PipeTransform {
  transform(value: string | null | undefined): string {
    if (!value || !/^\d{10}$/.test(value)) {
      return value ?? '';
    }

    return `0${value.slice(0, 3)} ${value.slice(3, 6)} ${value.slice(6, 8)} ${value.slice(8)}`;
  }
}
