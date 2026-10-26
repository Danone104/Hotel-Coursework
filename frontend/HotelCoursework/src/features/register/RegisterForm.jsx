import { useState } from "react"
import Field from "@/shared/components/Field"
import Button from "@/shared/components/Button"
import { loginApi } from "@/shared/api"


const RegisterForm = (props)=>{
    const {styles,setFormError, setShowAlert}= props
    const [noPatronymic,setNoPatronymic] = useState(false)
    const [registerformData, setRegisterFormData] = useState({
        name: '',
        surname: '',
        patronymic: '',
        phone: '',
        login: '',
        password: ''
    })
    const [errors, setErrors] = useState({
        name: '',
        surname: '',
        patronymic: '',
        phone: '',
        login: '',
        password:'',
    });
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
                
            case 'login':
                if (value && value.length < 3) {
                    error = 'Логин должен содержать минимум 3 символа';
                }else if(value.length >3){
                    const available = await loginApi.userNameAvailable(value)
                    if(!available){
                        error='Логин занят'
                    }
                }
                break;
        }
        }
        
        
        return error;
    }
    const handleCheckboxChange = ({target})=>{
        setNoPatronymic(target.checked)
        if(target.checked){
            setRegisterFormData(prev=> ({...prev,patronymic:''}))
            setErrors(prev => ({...prev,patronymic: ''}));
        }
    }

    const handleRegisterFormData = async ({target})=>{
        setRegisterFormData(prev => ({ ...prev, [target.id]: target.value.trimEnd()}))
        const error = await validateField(target.id, target.value.trimEnd());
        setErrors(prev => ({...prev,[target.id]: error}));
    }
    const handleFormChange = ()=>{
        setFormError('')
        setShowAlert(false)
    }
    const handleSubmitForm = async (event)=>{
        event.preventDefault()
        if(user){
            setFormError('Вы уже в аккаунте')
        }else{
        let isError=false
        for (let field in registerformData) {
            if(registerformData[field]===''){
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
            try{
                await loginApi.register(
                registerformData.login,
                registerformData.password,
                registerformData.phone,
                registerformData.name,
                registerformData.surname,
                registerformData.patronymic)
            }
            catch(error){
                setFormError(error.data)
            }
            
        }
        }
        
        setShowAlert(true)
        setRegisterFormData({
            name: '',
            surname: '',
            patronymic: '',
            phone: '',
            login: '',
            password: ''
        })
    }
    return(
        <form className={styles['register-form']} onChange={handleFormChange} onSubmit={handleSubmitForm}>
            <span className={styles['form-label']}>Регистрация</span>
            <div className={styles['loggining-inputs']}>
                <Field 
                label = 'Имя' 
                id='name'
                value={registerformData.name}
                onChange={handleRegisterFormData}
                error={errors.name}
                />
                <Field 
                label = 'Фамилия' 
                id='surname'
                value={registerformData.surname}
                onChange={handleRegisterFormData}
                error={errors.surname}
                />
                <div className={styles['patronymic']}>
                    <Field 
                    label = 'Отчество'
                    disabled = {noPatronymic}
                    value={registerformData.patronymic}
                    id='patronymic'
                    onChange={handleRegisterFormData}
                    error={errors.patronymic}
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
                value={registerformData.phone}
                id='phone'
                type = 'tel'
                onChange={handleRegisterFormData}
                error={errors.phone}
                />
                <Field 
                label = 'Логин' 
                id='login'
                value={registerformData.login}
                onChange={handleRegisterFormData}
                error={errors.login}
                />
                <Field 
                isPassword={true} 
                label = 'Пароль' 
                id='password'
                value={registerformData.password}
                onChange={handleRegisterFormData}
                error = {errors.password}
                />
            </div>
                
            <Button 
            className={styles['form-button']}
            type='submit'
            >Зарегистрироваться</Button>
        </form>
    )
}

export default RegisterForm