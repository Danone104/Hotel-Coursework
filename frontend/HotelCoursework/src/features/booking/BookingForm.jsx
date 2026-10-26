import { useEffect, useRef, useState } from 'react'
import styles from './BookingForm.module.css'
import Field from '../../shared/components/Field/Field'
import Button from '../../shared/components/Button/Button'
import { bookingApi } from '../../shared/api'
const BookingForm = (props)=>{
    const {isShown, onClose,room,dates,setBookingError,setShowBookingAlert} = props
    const [noPatronymic,setNoPatronymic] = useState(false)
    const [formError,setFormError]= useState(false)
        const [bookingFormData, setBookingFormData] = useState({
            name: '',
            surname: '',
            patronymic: '',
            phone: '',
        })
        const [errors, setErrors] = useState({
            name: '',
            surname: '',
            patronymic: '',
            phone: ''
        });
        const formRef = useRef(null)
        const validateField = async (name, value) => {
            let error = '';
            if(value.startsWith(' ')){
                error = 'Не может начинаться с пробела'
            }
            if(!error){
                switch (name) {
                case 'name':
                case 'surname':
                case 'patronymic':
                    if (/\d/.test(value)) {
                        error = 'Не может содержать цифры';
                        break
                    }
                    if(value&& value[0]!= value[0].toUpperCase()){
                        error = 'Должно начинаться с большой буквы'
                    }
                    if(value.length>1){
                        if(value.slice(1).toLowerCase()!==value.slice(1)){
                            error='Только первая дожна быть заглавной'
                        }
                    }
                    break;
                    
                case 'phone':
                    const phonePattern = /^\+7\d{10}$/;
                    if (value&& !phonePattern.test(value)) {
                        error = 'Формат: +7XXXXXXXXXX (10 цифр)';
                    }
                    break;
            }
            }
            
            
            return error;
        }
        const handleCheckboxChange = ({target})=>{
        setNoPatronymic(target.checked)
        if(target.checked){
            setBookingFormData(prev=> ({...prev,patronymic:''}))
            setErrors(prev => ({...prev,patronymic: ''}));
        }
    }

    const handleBookingFormData = async ({target})=>{
        setBookingFormData(prev => ({ ...prev, [target.id]: target.value.trimEnd()}))
        const error = await validateField(target.id, target.value.trimEnd());
        setErrors(prev => ({...prev,[target.id]: error}));
    }
    const handleFormChange=()=>{
        setFormError(null)
    }
    const handleSubmit = async (event)=>{
        event.preventDefault()
        let isError=false
                for (let field in bookingFormData) {
                    if(bookingFormData[field]===''){
                        if(field!=='patronymic' || (field==='patronymic'&& noPatronymic===false)){
                            setFormError('Все поля должны быть заполнены')
                            isError=true
                            break
                        }
                        
                    }
                }
                if(!isError){   
                    for (let error in errors) {
                    if(errors[error]!==''){
                        setFormError('Не все поля заполнены корректно')
                    }
                }}
                if(!isError){
                    const response = await bookingApi.booking(
                        bookingFormData.phone,
                        bookingFormData.name,
                        bookingFormData.surname,
                        bookingFormData.patronymic,
                        room.roomId,
                        dates.startDate,
                        dates.endDate
                    );
                    if(response) {
                        setBookingError(response.message);
                        setShowBookingAlert(true)
                        onClose()
                    } else {
                        setShowBookingAlert(true)
                        onClose()
                    }
                }
                
                setShowBookingAlert(true)
                setBookingFormData({
                    name: '',
                    surname: '',
                    patronymic: '',
                    phone: '',
                })
    }
    useEffect(() => {
        const handleClickOutside = (event) => {
            if (formRef.current && !formRef.current.contains(event.target)) {
                onClose()
            }
        }
        
        if (isShown) {
            document.addEventListener('pointerdown', handleClickOutside)
            document.body.style.overflow = 'hidden'
        }
        
        return () => {
            document.removeEventListener('pointerdown', handleClickOutside)
            document.body.style.overflow = 'unset'
        }
    }, [isShown, onClose])
    return(
        <div className={`${styles['overlay']} ${isShown ? '':styles['hidden']}`}>
            <form className={styles['booking-form']} onChange={handleFormChange} onSubmit={handleSubmit} ref={formRef}>
                        <div className={`${styles['alert']} ${formError? styles['error']:''}`}></div>
                        <span className={styles['form-label']}>Введите данные</span>
                        <div className={styles['form-inputs']}>
                            <Field 
                            label = 'Имя' 
                            id='name'
                            value={bookingFormData.name}
                            error={errors.name}
                            onChange={handleBookingFormData}
                            />
                            <Field 
                            label = 'Фамилия' 
                            id='surname'
                            value={bookingFormData.surname}
                            error={errors.surname}
                            onChange={handleBookingFormData}
                            />
                            <div className={styles['patronymic']}>
                                <Field 
                                label = 'Отчество'
                                disabled = {noPatronymic}
                                value={bookingFormData.patronymic}
                                id='patronymic'
                                error={errors.patronymic}
                                onChange={handleBookingFormData}
                                />
                                <Field 
                                type='checkbox' 
                                label='Отсутствует отчество'
                                value={noPatronymic}
                                onChange={handleCheckboxChange}
                                />
                            </div>
                            <Field 
                            label = 'Номер телефона'
                            value={bookingFormData.phone}
                            id='phone'
                            type = 'tel'
                            error={errors.phone}
                            onChange={handleBookingFormData}
                            />
                        </div>
                            
                        <Button 
                        className={styles['form-button']}
                        type='submit'
                        >Забронировать</Button>
                    </form>
        </div>
    )
}
export default BookingForm