import Swal from 'sweetalert2';

export const showLoadingAlert = () => {
  Swal.fire({
    title: 'Buscando...',
    text: 'Por favor, aguarde.',
    icon: 'info',
    allowOutsideClick: false,
    showConfirmButton: false,
    willOpen: () => {
      Swal.showLoading();
    },
    customClass: {
      popup: 'swal-custom-popup'
    }
  });
};

export const showErrorAlert = (message) => {
  Swal.fire({
    title: 'Erro',
    text: message,
    icon: 'error',
    confirmButtonText: 'Ok',
    customClass: {
      popup: 'swal-custom-popup'
    }
  });
};

export const showSuccessAlert = (message) => {
  Swal.fire({
    title: 'Sucesso',
    text: message,
    icon: 'success',
    confirmButtonText: 'Ok',
    customClass: {
      popup: 'swal-custom-popup'
    }
  });
};