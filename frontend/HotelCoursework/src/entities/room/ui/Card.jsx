import Button from "@/shared/components/Button"
import RouterLink from "@/shared/components/RouterLink/RouterLink"
import styles from './Card.module.css'
const Card = (props)=>{
    const {room}=props
    const URLImage = 'http://localhost:5262/api/rooms/room-'
    return(
       <div className={styles['card']}>
            <div className={styles['card__head']}>
                <img className={styles[['card__head-image']]} 
                src={`${URLImage}${room.roomId}/image`}
                 alt={`room-${room.roomId}`}/>
                <div className={styles["card__head-title"]}>
                    {room.title}
                </div>
            </div>
            <div className={styles["card__content"]}>
                <span className={styles["card__content-description"]}>
                    {room.description}
                </span>
                <div className={styles['card__content-info']}>
                    <span className={styles["count"]}>
                        {`${room.countPerson} человека`} 
                    </span>
                    <span className={styles["price"]}>
                        {`${room.price} р. за ночь`}
                    </span>
                </div>
            </div>
            <RouterLink to={`/booking/${room.roomId}`} className={styles['card__button-container']}>
                <Button className={`${styles['card__button']}`}>
                    Забронировать
                </Button>
            </RouterLink>
        </div>
            
    )
}

export default Card